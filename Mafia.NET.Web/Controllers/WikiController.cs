using Mafia.NET.Players.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.NET.Web.Controllers
{
    public class WikiController : BaseController
    {
        private static readonly RoleRegistry DefaultRoles = new RoleRegistry();

        public IActionResult Index(string role)
        {
            if (role == null) return View("Index");
            if (!DefaultRoles.Ids.ContainsKey(role)) return NotFound();
            ViewData["Role"] = DefaultRoles.Ids[role];
            return View("Wiki");
        }
    }
}