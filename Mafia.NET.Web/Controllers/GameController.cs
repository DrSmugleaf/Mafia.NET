using System;
using System.Collections.Generic;
using Mafia.NET.Matches;
using Mafia.NET.Web.Chat;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mafia.NET.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Dictionary<string, SessionPlayer> Players = new Dictionary<string, SessionPlayer>();
        private readonly Dictionary<string, ILobby> Lobbies = new Dictionary<string, ILobby>();
        private readonly Dictionary<string, IMatch> Matches = new Dictionary<string, IMatch>();
        
        public GameController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Lobby()
        {
            return View();
        }
        
        public IActionResult Game()
        {
            var id = HttpContext.Session.Id;
            if (Lobbies.ContainsKey(id)) throw new NotImplementedException();
            if (Matches.ContainsKey(id)) return View();
            
            return View("Join");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateGameViewModel model)
        {
            if (!ModelState.IsValid) return View("Join");

            return View("Join");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(CreateGameViewModel model)
        {
            if (!ModelState.IsValid) return View("Join");

            return View("Join");
        }
    }
}