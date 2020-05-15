using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Web.Controllers;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chat
{
    public class GameUser
    {
        public readonly string Id;
        public readonly string Connection;
        public readonly IPlayerController Player;

        public GameUser(string id, string connection, IPlayerController player)
        {
            Id = id;
            Connection = connection;
            Player = player;
        }
    }
    
    public class GameChat : Hub
    {
        private static readonly ConcurrentDictionary<Guid, GameUser> Users = new ConcurrentDictionary<Guid, GameUser>();

        public override Task OnConnectedAsync()
        {
            var guid = Guid.NewGuid();
            if (GameController.Entities.Players.TryGetValue(guid, out var player))
            {
                var id = Context.User.Identity.Name ?? throw new NullReferenceException("Name");
                var connection = Context.ConnectionId;
                var user = new GameUser(id, connection, player);

                Users.AddOrUpdate(guid, user, delegate { return user; });
                return base.OnConnectedAsync();
            }
            else
            {
                Context.Abort();
                throw new InvalidOperationException("No guid found for connection" + Context.ConnectionId);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.GetHttpContext().Session.TryGuid(out var guid))
                Users.TryRemove(guid, out _);
            
            return base.OnDisconnectedAsync(exception);
        }

        public Task NewMessage(string text)
        {
            if (!Context.GetHttpContext().Session.TryGuid(out var guid) ||
                !GameController.Entities.Players.TryGetValue(guid, out var player)) return Task.CompletedTask;
            
            var messages = player.Player.Match.Chat.Send(player.Player, text);

            foreach (var message in messages)
            {
                foreach (var listener in message.Listeners)
                {
                    var user = Users[listener.Guid()];
                    Clients.Client(user.Connection).SendAsync("Message", message.Formatted);
                }
            }

            return Task.CompletedTask;
        }
    }
}