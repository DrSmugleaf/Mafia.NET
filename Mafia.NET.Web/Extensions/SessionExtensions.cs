using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Mafia.NET.Players.Controllers;
using Microsoft.AspNetCore.Http;

namespace Mafia.NET.Web.Extensions
{
    public static class SessionExtensions
    {
        public static readonly ConcurrentDictionary<Guid, ILobbyController> LobbyControllers = new();

        public static readonly ConcurrentDictionary<Guid, IPlayerController> PlayerControllers = new();

        public static bool TryGuid(this ISession session, out Guid guid)
        {
            guid = default;
            if (session.TryGetValue("guid", out var bytes)) guid = new Guid(bytes);

            return guid != default;
        }

        public static void Guid(this ISession session, Guid guid)
        {
            if (session.TryLobbyController(out var lobby))
            {
                LobbyControllers[guid] = lobby;
                LobbyControllers.TryRemove(guid, out _);
            }

            if (session.TryPlayerController(out var player))
            {
                PlayerControllers[guid] = player;
                PlayerControllers.TryRemove(guid, out _);
            }

            session.Set("guid", guid.ToByteArray());
        }

        public static ILobbyController? LobbyController(this ISession session)
        {
            if (session.TryGuid(out var guid) && LobbyControllers.TryGetValue(guid, out var controller))
                return controller;

            return default;
        }

        public static bool TryLobbyController(this ISession session, [NotNullWhen(true)] out ILobbyController? controller)
        {
            controller = session.LobbyController();
            return controller != default;
        }

        public static void LobbyController(this ISession session, ILobbyController controller)
        {
            if (!session.TryGuid(out var guid) || guid != controller.Id)
                throw new ArgumentException($"Guid of session ({guid}) and controller ({controller.Id}) aren't equal.");

            LobbyControllers[guid] = controller;
        }

        public static IPlayerController? PlayerController(this ISession session)
        {
            if (session.TryGuid(out var guid) && PlayerControllers.TryGetValue(guid, out var controller))
                return controller;

            return default;
        }

        public static bool TryPlayerController(this ISession session, [NotNullWhen(true)] out IPlayerController? controller)
        {
            controller = session.PlayerController();
            return controller != default;
        }

        public static void PlayerController(this ISession session, IPlayerController controller)
        {
            if (!session.TryGuid(out var guid) || guid != controller.Id)
                throw new ArgumentException($"Guid of session ({guid}) and controller ({controller.Id}) aren't equal.");

            PlayerControllers[guid] = controller;
        }

        public static string? Connection(this ISession session)
        {
            return session.GetString("connection");
        }

        public static void Connection(this ISession session, string? connection)
        {
            if (connection == null)
            {
                session.Remove("connection");
                return;
            }

            session.SetString("connection", connection);
        }
    }
}