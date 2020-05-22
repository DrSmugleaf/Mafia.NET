using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class RoleEntry : IColorizable, ILocalizable
    {
        public RoleEntry(
            string id,
            ITeam team,
            IList<ICategory> categories,
            Color color,
            Color originalColor)
        {
            Id = id;
            Name = new Key($"{id}name");
            Summary = new Key($"{id}summary");
            Goal = new Key($"{id}goal");
            Abilities = new Key($"{id}abilities");
            Team = team;
            Categories = categories.ToImmutableList();
            Color = color;
            OriginalColor = originalColor;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key Abilities { get; }
        public ITeam Team { get; }
        public IImmutableList<ICategory> Categories { get; }
        public Color OriginalColor { get; }
        public Color Color { get; }

        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize();
        }
    }

    public class RoleRegistry
    {
        private static readonly Lazy<RoleRegistry> Lazy = new Lazy<RoleRegistry>(() => new RoleRegistry());

        private RoleRegistry()
        {
            var names = new Dictionary<string, RoleEntry>();
            var yamlRoles = Resource.FromDirectory("Roles", "*.yml");

            foreach (YamlMappingNode yaml in yamlRoles)
            {
                var id = yaml["id"].AsString();
                var team = (Team) yaml["team"].AsString();
                var categories = new List<ICategory>();
                var categoriesNode = yaml["categories"];

                if (categoriesNode is YamlScalarNode categoryNode)
                {
                    if (!categoryNode.IsNull()) categories.Add((Category) categoryNode.AsString());
                }
                else if (categoriesNode is YamlSequenceNode yamlCategories)
                {
                    foreach (var category in yamlCategories)
                        categories.Add((Category) category.AsString());
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized type for yamlCategories");
                }

                var color = yaml["color"].AsColor();
                var originalColor = yaml.Contains("original_color") ? yaml["original_color"].AsColor() : color;

                var role = new RoleEntry(id, team, categories, color, originalColor);
                names.Add(id, role);
            }

            Names = names.ToImmutableDictionary();
        }

        public static RoleRegistry Default => Lazy.Value;
        public IImmutableDictionary<string, RoleEntry> Names { get; }

        public List<RoleEntry> Get()
        {
            return Names.Values.ToList();
        }
        
        public List<RoleEntry> Get(params string[] names)
        {
            var roles = new List<RoleEntry>();

            foreach (var name in names)
            {
                var role = Names[name];
                roles.Add(role);
            }

            return roles;
        }

        public IRoleSelector Selector(string name)
        {
            return new RoleSelector(Names[name]);
        }

        public List<RoleSelector> Selectors(params string[] names)
        {
            return Get(names).Select(role => new RoleSelector(role)).ToList();
        }

        public List<RoleEntry> Category(ICategory category)
        {
            return Names.Values
                .Where(role => role.Categories.Contains(category))
                .OrderBy(role => role.Name.Localize()).ToList();
        }

        public List<RoleEntry> Team(ITeam team)
        {
            return Names.Values
                .Where(role => role.Team == team)
                .OrderBy(role => role.Name.Localize()).ToList();
        }
    }
}