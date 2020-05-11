using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Mafia.NET.Players.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mafia.NET.Web.Controllers
{
    public class HomeController : Controller
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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Wiki(string query)
        {
            if (query == null) return View();
            if (!RoleRegistry.Default.Names.ContainsKey(query)) return NotFound();
            ViewData["Role"] = RoleRegistry.Default.Names[query];
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}