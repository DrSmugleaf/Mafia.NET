using System.Collections.Concurrent;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public class EntityManager<T>
    {
        public ConcurrentDictionary<T, ILobbyController> Controllers { get; set; }
        public ConcurrentDictionary<T, IPlayerController> Players { get; set; }
        public ConcurrentDictionary<T, ILobby> Lobbies { get; set; }
        public ConcurrentDictionary<T, IMatch> Matches { get; set; }

        public EntityManager()
        {
            Controllers = new ConcurrentDictionary<T, ILobbyController>();
            Players = new ConcurrentDictionary<T, IPlayerController>();
            Lobbies = new ConcurrentDictionary<T, ILobby>();
            Matches = new ConcurrentDictionary<T, IMatch>();
        }
    }
}