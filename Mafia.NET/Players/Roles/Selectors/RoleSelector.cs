using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles.Selectors
{
    public interface IRoleSelector : IColorizable, ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        Key Summary { get; }
        Key Goal { get; }
        Key Abilities { get; }
        IImmutableList<RoleEntry> Possible { get; }
        HashSet<RoleEntry> Excluded { get; }
        bool Random => Possible.Count > 1;

        bool First(IList<RoleEntry> entry);
        bool TryResolve(Random random, IList<RoleEntry> resolved);
    }

    public class RoleSelector : IRoleSelector
    {
        public RoleSelector(
            string id,
            Key name,
            Key summary,
            Key goal,
            Key abilities,
            List<RoleEntry> possible,
            Color color)
        {
            Id = id;
            Name = name;
            Summary = summary;
            Goal = goal;
            Abilities = abilities;

            possible.RemoveAll(role => !role.Natural);
            Possible = possible.ToImmutableList();

            Excluded = new HashSet<RoleEntry>();
            Color = color;
        }

        public RoleSelector(RoleRegistry registry, ICategory category) : this(
            category.Id,
            category.Name,
            new ArgKey("SelectorCategoryDescription", string.Join(", ", registry.Category(category))),
            Key.Empty,
            Key.Empty,
            category.Roles(registry),
            Team.From(category.Team).Color)
        {
        }

        public RoleSelector(RoleEntry role) : this(
            role.Id,
            role.Name,
            role.Summary,
            role.Goal,
            role.AbilitiesDescription,
            new List<RoleEntry> {role},
            role.Color)
        {
        }

        public RoleSelector(RoleRegistry registry, ITeam team)
        {
            Id = Id;
            Name = new Key($"Selector{team.Id}Random");
            Summary = new Key($"Selector{team.Id}RandomDescription");
            Goal = Key.Empty;
            Abilities = Key.Empty;

            var possible = registry.Team(team);
            possible.RemoveAll(role => !role.Natural);
            Possible = possible.ToImmutableList();

            Excluded = new HashSet<RoleEntry>();
            Color = team.Color;
        }

        public RoleSelector(RoleRegistry registry) : this(
            "Any Random",
            SelectorKey.AnyRandom,
            SelectorKey.AnyRandomDescription,
            Key.Empty,
            Key.Empty,
            registry.Get(),
            ColorTranslator.FromHtml("#00CCFF"))
        {
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key Abilities { get; }
        public IImmutableList<RoleEntry> Possible { get; }
        public HashSet<RoleEntry> Excluded { get; }
        public Color Color { get; }

        public bool First(IList<RoleEntry> resolved)
        {
            var possible = new List<RoleEntry>(Possible);

            var unique = resolved.Where(role => role.Unique);
            foreach (var excluded in Excluded.Union(unique))
                possible.Remove(excluded);

            if (possible.Count != 1) return false;

            resolved.Add(possible[0]);
            return true;
        }

        public bool TryResolve(Random random, IList<RoleEntry> resolved)
        {
            var possible = new HashSet<RoleEntry>(Possible);

            var unique = resolved.Where(role => role.Unique);
            foreach (var excluded in Excluded.Union(unique))
                possible.Remove(excluded);

            if (possible.Count == 0) return false;

            resolved.Add(possible.ElementAt(random.Next(possible.Count)));
            return true;
        }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}