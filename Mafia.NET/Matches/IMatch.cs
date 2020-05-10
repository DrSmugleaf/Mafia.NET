using System;
using System.Collections.Generic;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        ISetup Setup { get; }
        IReadOnlyList<IPlayer> AllPlayers { get; }
        IReadOnlyList<IPlayer> LivingPlayers { get; }
        Graveyard Graveyard { get; }
        PhaseManager Phase { get; set; }
        ChatManager Chat { get; }
        Random Random { get; }

        void Start();
        void End();
    }
}