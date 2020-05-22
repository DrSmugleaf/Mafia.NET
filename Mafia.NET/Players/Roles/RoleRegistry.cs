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
            Color originalColor,
            bool natural)
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
            Natural = natural;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key Abilities { get; }
        public ITeam Team { get; }
        public IImmutableList<ICategory> Categories { get; }
        public Color OriginalColor { get; }
        public bool Natural { get; }
        public Color Color { get; }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }

    public class RoleRegistry
    {
        private static readonly Lazy<RoleRegistry> Lazy = new Lazy<RoleRegistry>(LoadAll);

        private RoleRegistry(IDictionary<string, RoleEntry> names)
        {
            Names = names.ToImmutableDictionary();
        }

        public static RoleRegistry Default => Lazy.Value;
        public IImmutableDictionary<string, RoleEntry> Names { get; }

        public static List<YamlMappingNode> LoadYaml()
        {
            var yaml = Resource.FromDirectory("Roles", "*.yml");
            return yaml.Select(resource => (YamlMappingNode) resource).ToList();
        }

        private static RoleRegistry LoadAll()
        {
            var names = new Dictionary<string, RoleEntry>();

            foreach (var yaml in LoadYaml())
            {
                var id = yaml["id"].AsString();
                var team = (Team) yaml["team"].AsString();
                var categories = new List<ICategory>();
                var categoryNames = yaml["categories"];

                foreach (var category in (YamlSequenceNode) categoryNames)
                    if (!category.IsNull())
                        categories.Add((Category) category.AsString());

                var color = yaml["color"].AsColor();
                var originalColor = yaml.Contains("original_color") ? yaml["original_color"].AsColor() : color;
                var natural = !yaml.Children.ContainsKey("natural") || yaml["natural"].AsBool();

                var role = new RoleEntry(id, team, categories, color, originalColor, natural);
                names.Add(id, role);
            }

            return new RoleRegistry(names);
        }

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
                .OrderBy(role => role.Id).ToList();
        }

        public List<RoleEntry> Team(ITeam team)
        {
            return Names.Values
                .Where(role => role.Team == team)
                .OrderBy(role => role.Id).ToList();
        }
    }
}