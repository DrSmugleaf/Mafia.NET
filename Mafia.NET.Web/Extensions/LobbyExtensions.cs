using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Web.Extensions;

public static class LobbyExtensions
{
    public static List<string> ControllerIds(this ILobby lobby)
    {
        return lobby.Controllers
            .Select(controller => controller.Id.ToString())
            .ToList();
    }

    public static List<string> IdsExcept(this ILobby lobby, ILobbyController except)
    {
        return lobby.Except(except)
            .Select(controller => controller.Id.ToString())
            .ToList();
    }
}