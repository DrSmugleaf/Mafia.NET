using System;
using Mafia.NET.Matches;

namespace Mafia.NET.Web.Extensions
{
    public static class LobbyExtensions
    {
        public static Guid Guid(this ILobby lobby)
        {
            return System.Guid.Parse(lobby.Id);
        }
    }
}