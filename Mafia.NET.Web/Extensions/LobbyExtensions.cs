using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Web.Chats;

namespace Mafia.NET.Web.Extensions
{
    public static class LobbyExtensions
    {
        public static Guid Guid(this ILobby lobby)
        {
            return System.Guid.Parse(lobby.Id);
        }

        public static IEnumerable<LobbyUser> All(this ILobby lobby)
        {
            return lobby.Controllers
                .Select(controller => LobbyChat.Users[controller.Guid()]);
        }

        public static IEnumerable<LobbyUser> Except(this ILobby lobby, ILobbyController except)
        {
            return lobby.Controllers
                .Where(controller => controller.Id != except.Id)
                .Select(controller => LobbyChat.Users[controller.Guid()]);
        }
    }
}