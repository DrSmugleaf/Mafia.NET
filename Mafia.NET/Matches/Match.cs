using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Matches
{
    public interface IMatch
    {
        Random Random { get; }
        Guid Id { get; }
        ISetup Setup { get; }
        IReadOnlyList<IPlayerController> Controllers { get; }
        IReadOnlyList<IPlayer> AllPlayers { get; }
        IReadOnlyList<IPlayer> LivingPlayers { get; }
        Graveyard Graveyard { get; }
        PhaseManager Phase { get; set; }
        ChatManager Chat { get; }
        RolePriority Priority { get; }
        event EventHandler<MatchEnd> MatchEnd;

        void Start();
        void Skip();
        void End();
        void OnEnd();
    }

    public class Match : IMatch
    {
        public Match(Guid id, ISetup setup, IList<ILobbyController> controllers)
        {
            Random = new Random();
            Id = id;
            Setup = setup;
            if (!setup.Roles.Randomize(Random, this, controllers, out var players))
                throw new ArgumentException($"Invalid setup {setup}");

            AllPlayers = players;
            Controllers = AllPlayers.Select(player => player.Controller).ToList();
            Graveyard = new Graveyard(this);
            Phase = new PhaseManager(this);
            Priority = new RolePriority(this);
        }

        public Random Random { get; }
        public Guid Id { get; }
        public ISetup Setup { get; }
        public IReadOnlyList<IPlayerController> Controllers { get; }
        public IReadOnlyList<IPlayer> AllPlayers { get; }
        public IReadOnlyList<IPlayer> LivingPlayers => new List<IPlayer>(AllPlayers.Where(player => player.Alive));
        public Graveyard Graveyard { get; }
        public PhaseManager Phase { get; set; }
        public ChatManager Chat => Phase.CurrentPhase.ChatManager;
        public RolePriority Priority { get; }
        public event EventHandler<MatchEnd> MatchEnd;

        public void Start()
        {
            foreach (var player in AllPlayers) player.Role.Ability.Initialize(player);

            Phase.Start();
        }

        public void Skip()
        {
            Phase.AdvancePhase();
        }

        public void End()
        {
            Phase.Close();
            OnEnd();
        }

        public void OnEnd()
        {
            MatchEnd?.Invoke(this, new MatchEnd(this));
        }
    }
}