using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
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
        public PhaseManager PhaseManager { get; set; }
        public ChatManager ChatManager => PhaseManager.CurrentPhase.ChatManager;

        public Match(ISetup settings, Dictionary<int, IPlayer> players, List<IRole> possibleRoles)
        {
            Setup = settings;
            AllPlayers = players;
            Graveyard = new Graveyard(this);
            PossibleRoles = possibleRoles;
            PhaseManager = new PhaseManager(this);
        }

        public void Start() => PhaseManager.Start();

        public void End() => PhaseManager.Close();
    }
}
