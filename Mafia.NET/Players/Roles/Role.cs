using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public interface IRole
    {
        string Name { get; }
        string Summary { get; }
        string Goal { get; }
        string Abilities { get; }
        ITeam Affiliation { get; }
        IReadOnlyList<ICategory> Categories { get; }
        Color Tint { get; }
        IAbility Ability { get; set; }

        bool IsCategory(string name);
        IReadOnlyList<Goal> Goals();
        IReadOnlyList<Goal> Enemies();
    }

    public class Role : IRole
    {
        public static readonly IReadOnlyDictionary<string, Role> Roles = LoadAll();
        public string Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public ITeam Affiliation { get; }
        public IReadOnlyList<ICategory> Categories { get; }
        public Color Tint { get; }
        public IAbility Ability { get; set; } // TODO

        public Role(string name, string summary, string goal, string abilities, ITeam affiliation, List<ICategory> categories, Color tint)
        {
            Name = name;
            Summary = summary;
            Goal = goal;
            Abilities = abilities;
            Affiliation = affiliation;
            Categories = categories.AsReadOnly();
            Tint = tint;
        }

        public static explicit operator Role(string name) => Roles[name];

        private static Dictionary<string, Role> LoadAll()
        {
            var roles = new Dictionary<string, Role>();
            var yamlRoles = Resource.FromDirectory("Roles", "*.yml");

            foreach (YamlMappingNode yaml in yamlRoles)
            {
                var name = yaml["name"].AsString();
                Console.WriteLine($"Parsing role {name}");

                Team affiliation = (Team)yaml["affiliation"].AsString();
                var categories = new List<ICategory>();
                YamlNode categoriesNode = yaml["categories"];

                if (categoriesNode is YamlScalarNode categoryNode)
                {
                    if (!categoryNode.IsNull())
                    {
                        categories.Add((Category)categoryNode.AsString());
                    }
                }
                else if (categoriesNode is YamlSequenceNode yamlCategories)
                {
                    foreach (var category in yamlCategories)
                    {
                        categories.Add((Category)category.AsString());
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized type for yamlCategories");
                }

                var tint = yaml["color"].AsColor();
                var summary = yaml["summary"].AsString();
                var goal = yaml["goal"].AsString();
                var abilities = yaml["ability"]["description"].AsString();

                var role = new Role(name, summary, goal, abilities, affiliation, categories, tint);
                roles.Add(name, role);
            }

            return roles;
        }

        public bool IsCategory(string name)
        {
            return Categories.Any(category => category.Name == name);
        }

        public IReadOnlyList<Goal> Goals() => Categories.Select(category => category.Goal).ToList();

        public IReadOnlyList<Goal> Enemies() => Categories.SelectMany(category => category.Goal.Enemies()).ToList();
    }
}
