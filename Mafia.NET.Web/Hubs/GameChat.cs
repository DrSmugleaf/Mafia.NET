using System;
using System.Linq;
using System.Threading.Tasks;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Hubs
{
    public class GameChat : Chat
    {
        public override Task OnConnectedAsync()
        {
            if (Session == null || !Session.TryPlayerController(out _))
            {
                Context.Abort();
                throw new InvalidOperationException(
                    $"No player controller found for connection {Context.ConnectionId}");
            }

            var connection = Context.UserIdentifier;
            Session.Connection(connection);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Session?.Connection(null);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewMessage(string text)
        {
            text = text.Trim();
            text = text.Substring(0, Math.Min(text.Length, 500));
            if (text.Length == 0 || Session == null || !Session.TryPlayerController(out var sender)) return;

            var messages = sender.Player.Match.Chat.Send(sender.Player, text);

            foreach (var message in messages)
            {
                var ids = message.Listeners
                    .Select(listener => listener.Id.ToString())
                    .ToList();
                await Clients.Users(ids).SendAsync("Message", message.Text);
            }
        }
    }
}