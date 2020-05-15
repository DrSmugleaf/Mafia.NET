using System;
using Microsoft.AspNetCore.Http;

namespace Mafia.NET.Web.Extensions
{
    public static class SessionExtensions
    {
        public static bool TryGuid(this ISession session, out Guid guid)
        {
            guid = default;
            if (session.TryGetValue("id", out var bytes)) guid = new Guid(bytes);

            return guid != default;
        }
    }
}