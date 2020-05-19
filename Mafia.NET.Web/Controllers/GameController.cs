using System;
using System.Collections.Concurrent;
using Mafia.NET.Matches;
using Mafia.NET.Web.Extensions;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mafia.NET.Web.Controllers
{
    public class GameController : BaseController
    {
        public static readonly ConcurrentDictionary<Guid, ILobby> Lobbies =
            new ConcurrentDictionary<Guid, ILobby>();

        public static readonly ConcurrentDictionary<Guid, IMatch> Matches =
            new ConcurrentDictionary<Guid, IMatch>();

        private readonly ILogger<HomeController> _logger;

        public GameController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Lobby()
        {
            if (!Session.TryLobbyController(out var controller)) return View("Join");

            var lobbyId = controller.Lobby.Id;
            ViewData["LobbyId"] = lobbyId;

            return View("Lobby");
        }

        public IActionResult Index()
        {
            if (Session.TryPlayerController(out _)) return Play();
            if (Session.TryLobbyController(out _)) return Lobby();

            return View("Join");
        }

        public IActionResult TestLobby()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") !=
                Environments.Development)
                return View("Join");

            var model = new JoinGameViewModel {Name = "Test Player"};
            return Create(model);
        }

        public IActionResult Play()
        {
            if (!Session.TryPlayerController(out var controller)) return View("Join");

            var player = controller.Player;
            ViewData["Players"] = player.Match.LivingPlayers;
            ViewData["Role"] = player.Role;
            ViewData["RoleList"] = player.Match.Setup.Roles.RoleList();

            return View("Game");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JoinGameViewModel model)
        {
            if (!ModelState.IsValid || !model.IsValidCreate()) return View("Join");

            var hostId = Guid.NewGuid();
            var lobbyId = Guid.NewGuid();
            var lobby = new Lobby(lobbyId, model.Name, hostId);
            Session.Guid(hostId);
            Session.LobbyController(lobby.Host);
            Lobbies[lobby.Id] = lobby;

            return Lobby();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinGameViewModel model)
        {
            if (!ModelState.IsValid ||
                !model.IsValidJoin() ||
                !Lobbies.TryGetValue(model.LobbyGuid(), out var lobby))
                return View("Join");

            var playerName = model.Name.Trim();
            var playerId = Guid.NewGuid();
            var player = lobby.Add(playerName, playerId);
            Session.Guid(playerId);
            Session.LobbyController(player);

            return Lobby();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Start(GameSettingsModel model)
        {
            if (!Session.TryLobbyController(out var host)) return View("Join");

            var lobby = host.Lobby;
            if (host.Id != lobby.Host.Id) return Lobby();

            if (!Lobbies.TryRemove(lobby.Id, out lobby)) return View("Join");

            lobby.Setup.Roles.MandatoryRoles = model.RoleEntries();
            var match = lobby.Start();
            Matches[lobby.Id] = match;

            foreach (var controller in lobby.Controllers)
                SessionExtensions.LobbyControllers.TryRemove(controller.Id, out _);

            foreach (var player in match.AllPlayers)
                SessionExtensions.PlayerControllers[player.Id] = player.Controller;

            return Play();
        }
    }
}