using System;
using System.Collections.Concurrent;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Web.Extensions;
using Mafia.NET.Web.Hubs;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Mafia.NET.Web.Controllers;

public class WebMatchManager : MatchManager
{
    private readonly IHubContext<GameChat> _hub;

    public WebMatchManager(IHubContext<GameChat> hub)
    {
        _hub = hub;
    }

    public override void OnChat(object? sender, Text notification)
    {
        var player = (IPlayer) sender!;

        _hub.Clients.User(player.Id.ToString()).SendAsync("Notification", notification); // TODO
    }

    public override void OnPopup(object? sender, Text notification)
    {
        var player = (IPlayer) sender!;

        _hub.Clients.User(player.Id.ToString()).SendAsync("Notification", notification); // TODO
    }
}

public class GameController : BaseController
{
    public static readonly ConcurrentDictionary<Guid, ILobby> Lobbies = new();

    private readonly IHubContext<GameChat> _gameContext;
    public readonly WebMatchManager Matches;

    public GameController(IHubContext<GameChat> gameContext)
    {
        _gameContext = gameContext;
        Matches = new WebMatchManager(gameContext);
    }

    public IActionResult Lobby()
    {
        if (!Session.TryLobbyController(out var controller)) return View("Join");

        var lobbyId = controller.Lobby.Id;
        ViewData["LobbyId"] = lobbyId.ToString("N");
        ViewData["SelectorGroups"] = SelectorGroup.Default();

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
        ViewData["RoleList"] = player.Match.Setup.Roles.Selectors;

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

        lobby.Setup.Roles.Selectors = model.RoleEntries();
        var match = lobby.Start();
        Matches.Add(match);

        foreach (var controller in lobby.Controllers)
            SessionExtensions.LobbyControllers.TryRemove(controller.Id, out _);

        foreach (var player in match.AllPlayers)
            SessionExtensions.PlayerControllers[player.Id] = player.Controller;

        return Play();
    }
}