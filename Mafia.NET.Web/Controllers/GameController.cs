using System;
using Mafia.NET.Matches;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
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
            if (HttpContext.Session.TryGetValue("id", out var id))
            {
                var guid = new Guid(id);
                
                if (Entities.Lobbies.ContainsKey(guid)) return Lobby();
                if (Entities.Matches.ContainsKey(guid)) return View("Game");
            }
            
            return View("Join");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JoinGameViewModel model)
        {
            if (!ModelState.IsValid || !model.IsValidCreate()) return View("Join");

            var lobby = new Lobby(model.Name);
            Entities.Lobbies.AddOrUpdate(new Guid(), lobby, (_, _2) => lobby);
            
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
            if (!ModelState.IsValid || !model.IsValidJoin() || !Entities.Lobbies.ContainsKey(model.GameGuid())) return View("Join");
            
            var lobby = Entities.Lobbies[model.GameGuid()];
            var player = lobby.Add(model.Name);
            var playerId = Guid.NewGuid();
            Entities.Controllers.TryAdd(playerId, player);

            return View("Lobby");
        }
    }
}