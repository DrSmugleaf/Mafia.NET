using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        IList<IPlayer> Graveyard { get; }
        IReadOnlyList<IRole> PossibleRoles { get; }
        TimePhase CurrentTime { get; set; }
        GamePhase CurrentPhase { get; set; }
        IList<IChat> OpenChats { get; }
    }
}
