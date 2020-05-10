using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class RoleEntry
    {
        public RoleEntry(
            string name,
            string summary,
            string goal,
            string abilities,
            ITeam affiliation,
            IList<ICategory> categories,
            Color tint)
        {
            Name = name;
            Summary = summary;
            Goal = goal;
            Abilities = abilities;
            Affiliation = affiliation;
            Categories = categories.ToImmutableList();
            Tint = tint;
        }

        public string Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public ITeam Affiliation { get; }
        public IImmutableList<ICategory> Categories { get; }
        public Color Tint { get; }
    }

    public class RoleRegistry
    {
        private static readonly Lazy<RoleRegistry> Lazy = new Lazy<RoleRegistry>(() => new RoleRegistry());

        private RoleRegistry()
        {
            var roles = new Dictionary<string, RoleEntry>();
            var yamlRoles = Resource.FromDirectory("Roles", "*.yml");

            foreach (YamlMappingNode yaml in yamlRoles)
            {
                var name = yaml["name"].AsString();
                Console.WriteLine($"Parsing role {name}");

                var affiliation = (Team) yaml["affiliation"].AsString();
                var categories = new List<ICategory>();
                var categoriesNode = yaml["categories"];

                if (categoriesNode is YamlScalarNode categoryNode)
                {
                    if (!categoryNode.IsNull()) categories.Add((Category) categoryNode.AsString());
                }
                else if (categoriesNode is YamlSequenceNode yamlCategories)
                {
                    foreach (var category in yamlCategories) categories.Add((Category) category.AsString());
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized type for yamlCategories");
                }

                var tint = yaml["color"].AsColor();
                var summary = yaml["summary"].AsString();
                var goal = yaml["goal"].AsString();
                var abilities = yaml["ability"]["description"].AsString();

                var role = new RoleEntry(name, summary, goal, abilities, affiliation, categories, tint);
                roles.Add(name, role);
            }

            Names = roles.ToImmutableDictionary();
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
    }
}