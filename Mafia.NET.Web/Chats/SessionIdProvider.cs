using System;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chats
{
    public class SessionIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var session = connection.GetHttpContext().Session;
            if (!session.TryGuid(out var guid))
            {
                guid = Guid.NewGuid();
                session.Guid(guid);
            }

            return guid.ToString();
        }
    }
}