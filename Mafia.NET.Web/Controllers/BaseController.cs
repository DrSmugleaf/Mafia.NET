using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.NET.Web.Controllers;

public class BaseController : Controller
{
    public ISession Session => HttpContext.Session;
}