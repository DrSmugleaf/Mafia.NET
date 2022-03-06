using System.Diagnostics;
using Mafia.NET.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.NET.Web.Controllers;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}