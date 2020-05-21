using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class RoleEntry : IColorizable, ILocalizable
    {
        public RoleEntry(
            string name,
            string summary,
            string goal,
            string abilities,
            ITeam team,
            IList<ICategory> categories,
            Color color,
            Color originalColor)
        {
            Id = name;
            Name = new Key($"{name}name");
            Summary = summary;
            Goal = goal;
            Abilities = abilities;
            Team = team;
            Categories = categories.ToImmutableList();
            Color = color;
            OriginalColor = originalColor;
        }

        public string Id { get; }
        public Key Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public ITeam Team { get; }
        public IImmutableList<ICategory> Categories { get; }
        public Color OriginalColor { get; }
        public Color Color { get; }

        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
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
                var name = yaml["name"].AsString();
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
                var summary = yaml["summary"].AsString();
                var goal = yaml["goal"].AsString();
                var abilities = yaml["ability"]["description"].AsString();

                var role = new RoleEntry(name, summary, goal, abilities, team, categories, color, originalColor);
                names.Add(name, role);
            }

            Names = names.ToImmutableDictionary();
        }

        public static RoleRegistry Default => Lazy.Value;
        public IImmutableDictionary<string, RoleEntry> Names { get; }

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

        public List<RoleEntry> Team(ITeam team)
        {
            return Names.Values.Where(role => role.Team == team)
                .OrderBy(role => role.Name.Localize()).ToList();
        }
    }
}