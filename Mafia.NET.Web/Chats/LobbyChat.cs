using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Web.Controllers;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chats
{
    public class LobbyUser
    {
        public readonly Guid Id;
        public readonly string Connection;
        public readonly ILobbyController Player;

        public LobbyUser(Guid id, string connection, ILobbyController player)
        {
            Id = id;
            Connection = connection;
            Player = player;
        }
    }
    
    public class LobbyChat : Hub
    {
        public static readonly ConcurrentDictionary<Guid, LobbyUser> Users = new ConcurrentDictionary<Guid, LobbyUser>();
        
        public override async Task OnConnectedAsync()
        {
            if (!Context.GetHttpContext().Session.TryGuid(out var guid))
                throw new NullReferenceException();
            
            var connection = Context.ConnectionId;
            var controller = GameController.Entities.Controllers[guid];
            var connected = new LobbyUser(guid, connection, controller);
            Users[guid] = connected;

            var lobby = controller.Lobby;
            var all = lobby.All().ToArray();
            
            if (connected.Player.Lobby.Host == connected.Player)
                await Clients.Client(connected.Connection).SendAsync("Host");

            var names = all.Select(user => user.Player.Name);
            await Clients.Client(connected.Connection)
                .SendAsync("Players", names);

            var others = lobby
                .Except(connected.Player)
                .Select(user => user.Connection)
                .ToArray();
            await Clients.Clients(others).SendAsync("Join", connected.Player.Name);

            var allConnections = all.Select(user => user.Connection).ToArray();
            await Clients.Clients(allConnections).SendAsync("HostPlayer", connected.Player.Lobby.Host.Name);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.GetHttpContext().Session.TryGuid(out var guid) && Users.TryRemove(guid, out var user))
            {
                var player = user.Player;
                var others = player.Lobby.Except(player)
                    .Select(other => other.Connection)
                    .ToArray();

                Clients.Clients(others).SendAsync("Leave", player.Name);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewMessage(string text)
        {
            text = text.Trim();
            text = text.Substring(0, Math.Min(text.Length, 500));
            if (text.Length == 0) return;
            
            if (Context.GetHttpContext().Session.TryGuid(out var guid) && GameController.Entities.Controllers.TryGetValue(guid, out var sender))
            {
                var users = sender.Lobby.Controllers
                    .Select(controller => Users[controller.Guid()].Connection)
                    .ToList()
                    .AsReadOnly();

                await Clients.Clients(users).SendAsync("Message", $"{sender.Name}: {text}");
            }
        }

        public async Task Start()
        {
            if (!Context.GetHttpContext().Session.TryGuid(out var guid)) return;

            var host = GameController.Entities.Controllers[guid];
            var lobby = host.Lobby;
            if (lobby.Started) return;
            
            var users = lobby.Controllers
                .Select(controller => Users[controller.Guid()].Connection)
                .ToList()
                .AsReadOnly();

            var match = lobby.Start();
            GameController.Entities.Lobbies.TryRemove(lobby.Guid(), out _);
            GameController.Entities.Matches[lobby.Guid()] = match;
            
            foreach (var controller in lobby.Controllers)
            {
                GameController.Entities.Controllers.TryRemove(controller.Guid(), out _);
            }
            
            foreach (var player in match.AllPlayers)
            {
                GameController.Entities.Players[player.Guid()] = player.Controller;
            }

            await Clients.Clients(users).SendAsync("Start");
        }
    }
}