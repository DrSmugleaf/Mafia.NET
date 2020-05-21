using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;

namespace Mafia.NET.Players.Roles
{
    public interface IRoleSelector : IColorizable, ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        string Summary { get; }
        string Goal { get; }
        string Abilities { get; }
        IImmutableList<RoleEntry> Possible { get; }
        HashSet<RoleEntry> Excludes { get; }

        bool TryResolve(Random random, out RoleEntry entry);
    }

    public class RoleSelector : IRoleSelector
    {
        public RoleSelector(
            string id,
            Key name,
            string summary,
            string goal,
            string abilities,
            List<RoleEntry> possible,
            Color color)
        {
            Id = id;
            Name = name;
            Summary = summary;
            Goal = goal;
            Abilities = abilities;
            Possible = possible.ToImmutableList();
            Excludes = new HashSet<RoleEntry>();
            Color = color;
        }

        public RoleSelector(RoleRegistry registry, ICategory category) : this(
            category.Id,
            category.Name,
            category.Description.Localize(),
            "",
            "",
            category.Possible(registry),
            category.Team.Color)
        {
        }
        
        public RoleSelector(RoleEntry role) : this(
            role.Id,
            role.Name,
            role.Summary,
            role.Goal,
            role.Abilities,
            new List<RoleEntry>() {role},
            role.Color)
        {
        }

        public string Id { get; }
        public Key Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public IImmutableList<RoleEntry> Possible { get; }
        public HashSet<RoleEntry> Excludes { get; }
        public Color Color { get; }

        public bool TryResolve(Random random, [CanBeNull, NotNullWhen(true)] out RoleEntry entry)
        {
            entry = default;
            var possible = new HashSet<RoleEntry>(Possible);
            
            foreach (var excluded in Excludes)
            {
                possible.Remove(excluded);
            }

            if (possible.Count > 0)
                entry = possible.ElementAt(random.Next(possible.Count()));

            return entry != default;
        }
        
        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize();
        }
    }
}