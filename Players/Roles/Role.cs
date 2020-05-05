using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class Role : IRole
    {
        public static readonly IReadOnlyDictionary<string, Role> Roles = LoadAll();
        public string Name { get; }
        public ITeam Affiliation { get; }
        public IReadOnlyList<ICategory> Categories { get; }
        public Color Tint { get; }

        public Role(string name, ITeam affiliation, List<ICategory> categories, Color tint)
        {
            Name = name;
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

                var role = new Role(name, affiliation, categories, tint);
                roles.Add(name, role);
            }

            return roles;
        }
    }
}
