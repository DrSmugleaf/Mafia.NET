﻿using System;
using System.Collections.Generic;
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
            return View();
        }
        
        public IActionResult Index()
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            if (Entities.Controllers.ContainsKey(guid)) return Lobby();
            if (Entities.Matches.ContainsKey(guid)) return View("Game");

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
            
            return View("Lobby");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JoinGameViewModel model)
        {
            var hasId = HttpContext.Session.TryGuid(out var guid);
            if (!ModelState.IsValid || !model.IsValidCreate() || !hasId) return View("Join");

            var lobbyId = Guid.NewGuid();
            var lobby = new Lobby(lobbyId.ToString("N"), model.Name, guid.ToString("N"));
            Entities.Lobbies.AddOrUpdate(lobbyId, lobby, (_, _2) => lobby);
            
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
            if (!ModelState.IsValid || !model.IsValidJoin())
                return View("Join");

            var lobby = Entities.Lobbies[model.GameGuid()];
            var playerId = Guid.NewGuid();
            var player = lobby.Add(model.Name, playerId.ToString("N"));
            
            Entities.Controllers[playerId] = player;
            HttpContext.Session.Set("id", playerId.ToByteArray());

            return View("Lobby");
        }

        public IActionResult Start(StartGameModel model)
        {
            if (!HttpContext.Session.TryGuid(out var guid)) return View("Join");
            var host = Entities.Controllers[guid];
            var lobby = host.Lobby;
            lobby.Start();
            
            return View("Game");
        }
    }
}