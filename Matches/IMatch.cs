using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;
using System.Timers;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        List<IDeath> Graveyard { get; }
        IList<IDeath> UndisclosedDeaths { get; }
        IReadOnlyList<IRole> PossibleRoles { get; }
        TimePhase CurrentTime { get; set; }
        IPhase CurrentPhase { get; set; }
        IList<IChat> OpenChats { get; }
        Timer Timer { get; }

        void AdvancePhase(object state);
        void End();
    }
}
