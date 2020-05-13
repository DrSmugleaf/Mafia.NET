using System.Collections.Concurrent;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public class EntityManager<T>
    {
        public ConcurrentDictionary<T, IPlayerController> Controllers { get; set; }
        public ConcurrentDictionary<T, IPlayer> Players { get; set; }
        public ConcurrentDictionary<T, ILobby> Lobbies { get; set; }
        public ConcurrentDictionary<T, IMatch> Matches { get; set; }

        public EntityManager()
        {
            Controllers = new ConcurrentDictionary<T, IPlayerController>();
            Players = new ConcurrentDictionary<T, IPlayer>();
            Lobbies = new ConcurrentDictionary<T, ILobby>();
            Matches = new ConcurrentDictionary<T, IMatch>();
        }
    }
}