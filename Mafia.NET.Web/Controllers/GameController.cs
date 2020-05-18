using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Web.Extensions;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mafia.NET.Web.Controllers
{
    public class GameController : Controller
    {
        public static readonly EntityManager<Guid> Entities = new EntityManager<Guid>();
        private readonly ILogger<HomeController> _logger;
        
        public GameController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Lobby()
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            var lobbyId = Entities.Controllers[guid].Lobby.Guid().ToString("N");
            ViewData["LobbyId"] = lobbyId;
            
            return View("Lobby");
        }
        
        public IActionResult Index()
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            if (Entities.Matches.ContainsKey(guid)) return Play();
            if (Entities.Controllers.ContainsKey(guid)) return Lobby();

            return View("Join");
        }
        
        public IActionResult TestLobby()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Development)
                return View("Join");

            var name = "Test Player";
            var hostId = Guid.NewGuid();
            var lobbyId = Guid.NewGuid();
            var lobby = new Lobby(lobbyId.ToString("N"), name, hostId.ToString("N"));
            Entities.Lobbies[lobbyId] = lobby;
            Entities.Controllers[hostId] = lobby.Host;
            HttpContext.Session.Set("id", hostId.ToByteArray());
            ViewData["LobbyId"] = lobbyId.ToString("N");

            return View("Lobby");
        }

        public IActionResult Play()
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            if (!Entities.Players.ContainsKey(guid)) return View("Join");
            
            var player = Entities.Players[guid].Player;
            
            ViewData["Players"] = player.Match.LivingPlayers;
            ViewData["Role"] = player.Role;
            ViewData["RoleList"] = player.Match.Setup.Roles.RoleList();

            return View("Game");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JoinGameViewModel model)
        {
            var hasId = HttpContext.Session.TryGuid(out var guid);
            if (!ModelState.IsValid || !model.IsValidCreate() || !hasId) return View("Join");

            var lobbyId = Guid.NewGuid();
            var lobby = new Lobby(lobbyId.ToString("N"), model.Name, guid.ToString("N"));
            Entities.Lobbies.AddOrUpdate(lobbyId, lobby, delegate { return lobby; });
            
            var playerId = Guid.NewGuid();
            HttpContext.Session.Set("id", playerId.ToByteArray());
            
            var host = lobby.Host;
            Entities.Controllers.TryAdd(playerId, host);
            
            return View("Lobby");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinGameViewModel model)
        {
            if (!ModelState.IsValid || !model.IsValidJoin() || !Entities.Lobbies.ContainsKey(model.GameGuid()))
                return View("Join");

            var lobby = Entities.Lobbies[model.GameGuid()];
            var playerName = model.Name.Trim();
            var playerId = Guid.NewGuid();
            var player = lobby.Add(playerName, playerId.ToString("N"));
            
            Entities.Controllers[playerId] = player;
            HttpContext.Session.Set("id", playerId.ToByteArray());

            return Lobby();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Start(GameSettingsModel model)
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            
            var host = Entities.Controllers[guid];
            if (host.Guid() != guid) return Lobby();

            var lobby = host.Lobby;
            lobby.Setup.Roles.MandatoryRoles = model.RoleEntries();
            var match = lobby.Start();
            Entities.Lobbies.TryRemove(lobby.Guid(), out _);
            Entities.Matches[lobby.Guid()] = match;
            
            foreach (var controller in lobby.Controllers)
                Entities.Controllers.TryRemove(controller.Guid(), out _);
            
            foreach (var player in match.AllPlayers)
                Entities.Players[player.Guid()] = player.Controller;
            
            return Play();
        }
    }
}