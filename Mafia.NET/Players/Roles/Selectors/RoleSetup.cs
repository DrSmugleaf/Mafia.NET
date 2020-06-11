using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Selectors
{
    public class RoleSetup
    {
        public RoleSetup(
            RoleRegistry roles,
            AbilityRegistry abilities,
            AbilitySetupRegistry abilitySetups,
            IEnumerable<IRoleSelector> selectors)
        {
            Roles = roles;
            Abilities = abilities;
            AbilitySetups = abilitySetups;
            Perks = new PerkRegistry(roles);
            Selectors = selectors.ToList();
        }

        public RoleSetup(IEnumerable<IRoleSelector> selectors) : this(
            RoleRegistry.Default,
            new AbilityRegistry(),
            new AbilitySetupRegistry(),
            selectors.ToList())
        {
        }

        public RoleSetup(params string[] roles) : this(
            RoleRegistry.Default,
            new AbilityRegistry(),
            new AbilitySetupRegistry(),
            RoleRegistry.Default.Selectors(roles))
        {
        }

        public RoleSetup() : this(new List<IRoleSelector>())
        {
        }

        public RoleRegistry Roles { get; }
        public AbilityRegistry Abilities { get; }
        public AbilitySetupRegistry AbilitySetups { get; }
        public PerkRegistry Perks { get; }
        public List<IRoleSelector> Selectors { get; set; }

        public int Players()
        {
            return Selectors.Count;
        }

        public List<IPlayer> Static(IMatch match)
        {
            var players = new List<IPlayer>();
            var roleEntries = new List<RoleEntry>();

            foreach (var selector in Selectors)
                if (!selector.First(roleEntries))
                    throw new ArgumentException($"No first role found for selector {selector}");

            for (var i = 0; i < roleEntries.Count; i++)
            {
                var roleEntry = roleEntries[i];
                var role = new Role(roleEntry);
                var controller = new LobbyController($"Bot {i + 1}", null);
                var player = new Player(controller, match, i + 1, controller.Name, role);

                players.Add(player);
            }

            return players;
        }

        public bool Randomize(Random random, out List<RoleEntry> roles)
        {
            roles = new List<RoleEntry>();

            foreach (var selector in Selectors)
                if (!selector.TryResolve(random, roles))
                    return false;

            return true;
        }

        public bool Randomize(Random random, IMatch match, IList<ILobbyController> controllers,
            out List<IPlayer> players)
        {
            players = new List<IPlayer>();
            if (!Randomize(random, out var roles)) return false;

            if (controllers.Count != roles.Count)
                throw new ArgumentException(
                    $"Size of controllers ({controllers.Count}) doesn't equal size of roles ({roles.Count}).");

            for (var i = 0; i < controllers.Count; i++)
            {
                var controller = controllers[i];
                var roleEntry = roles[i];
                var role = new Role(roleEntry);
                var player = new Player(controller, match, i + 1, controller.Name, role);

                players.Add(player);
            }

            return true;
        }
    }
}