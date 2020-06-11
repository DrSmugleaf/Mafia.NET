using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Roles.Selectors;

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
        RoleSetup RoleSetup { get; }
        RoleRegistry Roles { get; }
        PerkRegistry Perks { get; }
        AbilityRegistry Abilities { get; }
        AbilitySetupRegistry AbilitySetups { get; }
        AbilityExecutor Executor { get; }
        event EventHandler<MatchEnd> MatchEnd;

        void Start();
        void Skip();
        T Skip<T>() where T : IPhase;
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

            AllPlayers = players.OrderBy(player => player.Number).ToList();
            Controllers = AllPlayers.Select(player => player.Controller).ToList();
            Graveyard = new Graveyard(this);
            Phase = new PhaseManager(this);
        }

        public Match(params string[] roles)
        {
            Random = new Random();
            Id = Guid.NewGuid();

            var roleSetup = new RoleSetup(roles);
            Setup = new Setup(roleSetup);

            AllPlayers = Setup.Roles.Static(this);
            Controllers = AllPlayers.Select(player => player.Controller).ToList();
            Graveyard = new Graveyard(this);
            Phase = new PhaseManager(this);
            Executor = new AbilityExecutor(this);
        }

        public Random Random { get; }
        public Guid Id { get; }
        public ISetup Setup { get; }
        public IReadOnlyList<IPlayerController> Controllers { get; }
        public IReadOnlyList<IPlayer> AllPlayers { get; }

        public IReadOnlyList<IPlayer> LivingPlayers => AllPlayers
            .Where(player => player.Alive)
            .ToList();

        public Graveyard Graveyard { get; }
        public PhaseManager Phase { get; set; }
        public ChatManager Chat => Phase.Chat;
        public RoleSetup RoleSetup => Setup.Roles;
        public RoleRegistry Roles => RoleSetup.Roles;
        public PerkRegistry Perks => RoleSetup.Perks;
        public AbilityRegistry Abilities => RoleSetup.Abilities;
        public AbilitySetupRegistry AbilitySetups => RoleSetup.AbilitySetups;
        public AbilityExecutor Executor { get; }
        public event EventHandler<MatchEnd> MatchEnd;

        public void Start()
        {
            foreach (var player in AllPlayers) player.Role.Initialize(player);
            Phase.Start();
        }

        public void Skip()
        {
            Phase.AdvancePhase();
        }

        public T Skip<T>() where T : IPhase
        {
            while (!(Phase.CurrentPhase is T)) Skip();
            return (T) Phase.CurrentPhase;
        }

        public void End()
        {
            Chat.Close();
            Phase.Close();
            OnEnd();
        }

        public void OnEnd()
        {
            MatchEnd?.Invoke(this, new MatchEnd(this));
        }
    }
}