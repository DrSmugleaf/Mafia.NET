using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches
{
    public class Match : IMatch
    {
        public ISetup Setup { get; }
        public IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        public IReadOnlyDictionary<int, IPlayer> LivingPlayers => new Dictionary<int, IPlayer>(AllPlayers.Where(player => player.Value.Alive));
        public Graveyard Graveyard { get; }
        public IReadOnlyList<IRole> PossibleRoles { get; }
        public PhaseManager Phase { get; set; }
        public ChatManager Chat => Phase.CurrentPhase.ChatManager;
        public AbilityRegistry Abilities { get; set; }
        public Random Random { get; }

        public Match(ISetup settings, Dictionary<int, IPlayer> players, List<IRole> possibleRoles, AbilityRegistry abilities = null)
        {
            Setup = settings;
            AllPlayers = players;
            Graveyard = new Graveyard(this);
            PossibleRoles = possibleRoles;
            Phase = new PhaseManager(this);
            Abilities = abilities ?? AbilityRegistry.Default;
            Random = new Random();
        }

        public void Start() => Phase.Start();

        public void End() => Phase.Close();
    }
}
