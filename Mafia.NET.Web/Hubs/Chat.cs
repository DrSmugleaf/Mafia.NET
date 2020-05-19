using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Hubs
{
    public abstract class Chat : Hub
    {
        public ISession Session => Context.GetHttpContext().Session;
    }
}