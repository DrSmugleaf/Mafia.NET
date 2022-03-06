using System;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Hubs;

public class SessionIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var session = connection.GetHttpContext()?.Session ?? throw new NullReferenceException();
        if (!session.TryGuid(out var guid))
        {
            guid = Guid.NewGuid();
            session.Guid(guid);
        }

        return guid.ToString();
    }
}