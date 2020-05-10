using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;

namespace Mafia.NET.Players.Roles
{
#nullable enable
    public class RoleSetup
    {
        private static readonly Random Random = new Random();

        public RoleSetup(
            RoleRegistry roles,
            AbilityRegistry abilities,
            IEnumerable<RoleEntry>? mandatoryRoles = null,
            IEnumerable<ICategory>? mandatoryCategories = null)
        {
            Roles = roles;
            Abilities = abilities;
            MandatoryRoles = mandatoryRoles == null ? new List<RoleEntry>() : mandatoryRoles.ToList();
            MandatoryCategories = mandatoryCategories == null ? new List<ICategory>() : mandatoryCategories.ToList();
        }

        public RoleSetup(
            IEnumerable<RoleEntry>? mandatoryRoles = null,
            IEnumerable<ICategory>? mandatoryCategories = null) :
            this(RoleRegistry.Default, AbilityRegistry.Default, mandatoryRoles, mandatoryCategories)
        {
        }

        public RoleRegistry Roles { get; }
        public AbilityRegistry Abilities { get; }
        public List<RoleEntry> MandatoryRoles { get; }
        public List<ICategory> MandatoryCategories { get; }

        public int Players()
        {
            return MandatoryRoles.Count + MandatoryCategories.Count;
        }

        public HashSet<RoleEntry> Possible()
        {
            var possible = new HashSet<RoleEntry>(MandatoryRoles);
            foreach (var category in MandatoryCategories)
            {
                var categoryRoles = Roles.Names.Values
                    .Where(role => role.Categories.Contains(category));
                var roles = categoryRoles.ToList();
                var role = roles[Random.Next(roles.Count())];

                possible.Add(role);
            }

            return possible;
        }

        public List<RoleEntry> Randomize()
        {
            var roles = new List<RoleEntry>(MandatoryRoles);
            foreach (var category in MandatoryCategories)
            {
                var categoryRoles = Roles.Names.Values.Where(role => role.Categories.Contains(category));
                var role = categoryRoles.ElementAt(Random.Next(categoryRoles.Count()));
                roles.Add(role);
            }

            return roles;
        }

        public List<IPlayer> Randomize(IList<IController> controllers, IMatch match)
        {
            var players = new List<IPlayer>();
            var roles = Randomize();

            if (controllers.Count != roles.Count)
                throw new ArgumentException(
                    $"Size of controllers ({controllers.Count}) doesn't equal size of roles ({roles.Count}).");

            for (var i = 0; i < controllers.Count; i++)
            {
                var controller = controllers[i];
                var roleEntry = roles[i];
                var ability = Abilities.Names[roleEntry.Name].Build();
                var role = new Role(roleEntry, ability);
                var player = controller.Player(match, i + 1, role);
                players.Add(player);
            }

            return players;
        }
    }
}