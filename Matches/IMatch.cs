﻿using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        ISetup Setup { get; }
        IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        IReadOnlyDictionary<int, IPlayer> LivingPlayers { get; }
        List<IDeath> Graveyard { get; }
        IList<IDeath> UndisclosedDeaths { get; }
        IReadOnlyList<IRole> PossibleRoles { get; }
        PhaseManager PhaseManager { get; set; }

        bool AnyDiedToday(DeathCause cause)
        {
            return Graveyard.Any(death => death.Day == PhaseManager.Day && death.Cause == cause);
        }
        void End();
    }
}
