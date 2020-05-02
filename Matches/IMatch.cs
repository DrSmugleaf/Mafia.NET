using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
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
        ISettings Settings { get; }
        IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        IReadOnlyDictionary<int, IPlayer> LivingPlayers { get; }
        List<IDeath> Graveyard { get; }
        IList<IDeath> UndisclosedDeaths { get; }
        IReadOnlyList<IRole> PossibleRoles { get; }
        TimePhase CurrentTime { get; set; }
        IPhase CurrentPhase { get; set; }
        IList<IChat> OpenChats { get; }
        Timer Timer { get; }

        void SupersedePhase(IPhase newPhase);
        void AdvancePhase(object state);
        void End();
    }
}
