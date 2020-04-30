using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;

namespace Mafia.NET.Matches
{
    interface IMatch
    {
        public IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        public IList<IPlayer> Graveyard { get; }
        public IReadOnlyList<IRole> PossibleRoles { get; }
        public TimePhase CurrentTime { get; set; }
        public GamePhase CurrentPhase { get; set; }
        public IList<IChat> OpenChats { get; }
    }
}
