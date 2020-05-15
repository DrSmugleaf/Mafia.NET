using System;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static Guid Guid(this ILobbyController player)
        {
            return System.Guid.Parse(player.Id);
        }
        
        public static Guid Guid(this IPlayerController player)
        {
            return System.Guid.Parse(player.Id);
        }
    }
}