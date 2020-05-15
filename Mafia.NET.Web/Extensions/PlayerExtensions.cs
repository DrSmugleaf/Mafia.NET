using System;
using Mafia.NET.Players;

namespace Mafia.NET.Web.Extensions
{
    public static class PlayerExtensions
    {
        public static Guid Guid(this IPlayer player)
        {
            return System.Guid.Parse(player.Id);
        }
    }
}