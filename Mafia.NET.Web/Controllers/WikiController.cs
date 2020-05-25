using Mafia.NET.Players.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.NET.Web.Controllers
{
    public class WikiController : BaseController
    {
        public IActionResult Index(string role)
        {
            if (role == null) return View("Index");
            if (!RoleRegistry.Default.Names.ContainsKey(role)) return NotFound();
            ViewData["Role"] = RoleRegistry.Default.Names[role];
            return View("Wiki");
        }
    }
}