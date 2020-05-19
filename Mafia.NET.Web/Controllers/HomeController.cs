using System.Diagnostics;
using Mafia.NET.Players.Roles;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mafia.NET.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Wiki(string query)
        {
            if (query == null) return View("WikiIndex");
            if (!RoleRegistry.Default.Names.ContainsKey(query)) return NotFound();
            ViewData["Role"] = RoleRegistry.Default.Names[query];
            return View("Wiki");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}