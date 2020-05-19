using System;
using System.Linq;
using System.Threading.Tasks;
using Mafia.NET.Web.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Mafia.NET.Web.Chats
{
    public class LobbyChat : Chat
    {
        public override async Task OnConnectedAsync()
        {
            if (!Session.TryLobbyController(out var connected))
            {
                Context.Abort();
                throw new InvalidOperationException("No lobby controller found for connection" + Context.ConnectionId);
            }

            var connection = Context.UserIdentifier;
            Session.Connection(connection);

            var lobby = connected.Lobby;

            if (connected.Lobby.Host == connected)
                await Clients.User(connection).SendAsync("Host");

            var names = lobby.Controllers
                .Select(user => user.Name);
            await Clients.User(connection).SendAsync("Players", names);

            var others = lobby.IdsExcept(connected);
            await Clients.Users(others).SendAsync("Join", connected.Name);

            var allConnections = connected.Lobby.ControllerIds();
            await Clients.Users(allConnections).SendAsync("HostPlayer", connected.Lobby.Host.Name);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Session.TryLobbyController(out var controller))
            {
                var others = controller.Lobby.IdsExcept(controller);
                Clients.Users(others).SendAsync("Leave", controller.Name);
            }

            Session.Connection(null);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task NewMessage(string text)
        {
            text = text.Trim();
            text = text.Substring(0, Math.Min(text.Length, 500));
            if (text.Length == 0 || !Session.TryLobbyController(out var sender)) return;

            var users = sender.Lobby.ControllerIds();
            await Clients.Users(users).SendAsync("Message", $"{sender.Name}: {text}");
        }

        public async Task Start()
        {
            if (Session.TryLobbyController(out var host)) return;

            var lobby = host.Lobby;
            if (lobby.Started || host != lobby.Host) return;

            var match = lobby.Start();

            foreach (var controller in lobby.Controllers)
                SessionExtensions.LobbyControllers.TryRemove(controller.Id, out _);

            foreach (var player in match.AllPlayers)
                SessionExtensions.PlayerControllers[player.Id] = player.Controller;

            await Clients.Users(lobby.ControllerIds()).SendAsync("Start");
        }
    }
}