using System.Threading.Tasks;
using Mafia.NET.Web.Controllers;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chat
{
    public class LobbyChat : Hub
    {
        public async Task NewMessage(string message)
        {
            if (Context.GetHttpContext().Session.TryGuid("id", out var guid) && GameController.Entities.Controllers.TryGetValue(guid, out var controller))
            {
                await Clients.All.SendAsync("messageReceived", controller.Name, message);
            }
        }
    }
}