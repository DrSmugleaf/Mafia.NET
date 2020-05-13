using System;
using Mafia.NET.Players.Controllers;
using Microsoft.AspNetCore.Http;

namespace Mafia.NET.Web.Extensions
{
    public static class SessionExtensions
    {
        public static bool TryGuid(this ISession session, string key, out Guid guid)
        {
            guid = default;
            if (session.TryGetValue(key, out var bytes))
            {
                guid = new Guid(bytes);
            }

            return guid != null;
        }
    }
}