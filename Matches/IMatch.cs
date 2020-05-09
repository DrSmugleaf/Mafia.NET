using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        ISetup Setup { get; }
        IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        IReadOnlyDictionary<int, IPlayer> LivingPlayers { get; }
        Graveyard Graveyard { get; }
        IReadOnlyList<IRole> PossibleRoles { get; }
        PhaseManager Phase { get; set; }
        ChatManager Chat { get; }
        AbilityRegistry Abilities { get; }
        Random Random { get; }

        void End();
    }
}
