using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;

namespace Mafia.NET.Matches
{
    public class Match : IMatch
    {
        public IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        public IList<IPlayer> Graveyard { get; }
        public IReadOnlyList<IRole> PossibleRoles { get; }
        public TimePhase CurrentTime { get; set; }
        public GamePhase CurrentPhase { get; set; }
        public IList<IChat> OpenChats { get; }

        public Match(Dictionary<int, IPlayer> players, List<IRole> possibleRoles)
        {
            AllPlayers = players;
            Graveyard = new List<IPlayer>();
            PossibleRoles = possibleRoles;
            CurrentTime = TimePhase.DAY;
            CurrentPhase = GamePhase.STARTING;
            OpenChats = new List<IChat>();
        }
    }
}
