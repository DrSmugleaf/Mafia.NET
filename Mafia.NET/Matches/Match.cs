using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        ISetup Setup { get; }
        IReadOnlyList<IPlayerController> Controllers { get; }
        IReadOnlyList<IPlayer> AllPlayers { get; }
        IReadOnlyList<IPlayer> LivingPlayers { get; }
        Graveyard Graveyard { get; }
        PhaseManager Phase { get; set; }
        ChatManager Chat { get; }
        Random Random { get; }

        void Start();
        void Skip();
        void End();
    }

    public class Match : IMatch
    {
        public Match(ISetup setup, IList<ILobbyController> controllers)
        {
            Setup = setup;
            AllPlayers = setup.Roles.Randomize(controllers, this);
            Controllers = AllPlayers.Select(player => player.Controller).ToList();
            Graveyard = new Graveyard(this);
            Phase = new PhaseManager(this);
            Random = new Random();
        }

        public ISetup Setup { get; }
        public IReadOnlyList<IPlayerController> Controllers { get; }
        public IReadOnlyList<IPlayer> AllPlayers { get; }
        public IReadOnlyList<IPlayer> LivingPlayers => new List<IPlayer>(AllPlayers.Where(player => player.Alive));
        public Graveyard Graveyard { get; }
        public PhaseManager Phase { get; set; }
        public ChatManager Chat => Phase.CurrentPhase.ChatManager;
        public Random Random { get; }

        public void Start()
        {
            foreach (var player in AllPlayers) player.Role.Ability.Initialize(this, player);

            Phase.Start();
        }

        public void Skip()
        {
            Phase.AdvancePhase();
        }

        public void End()
        {
            Phase.Close();
        }
    }
}