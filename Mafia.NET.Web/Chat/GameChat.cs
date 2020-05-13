using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chat
{
    public class GameChat : Hub
    {
        public async Task NewMessage(long username, string message)
        {
            await Clients.All.SendAsync("messageReceived", username, message);
        }
    }
}