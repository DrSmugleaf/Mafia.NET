using Mafia.NET.Extension;
using Mafia.NET.Player.Teams;
using Mafia.NET.Resources;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Player.Roles
{
    class Role : IRole
    {
        public static readonly IReadOnlyDictionary<string, Role> Roles = LoadAll();
        public string Name { get; }
        public ITeam Affiliation { get; }
        public IReadOnlyList<string> Categories { get; }

        public Role(string name, ITeam affiliation, List<string> categories)
        {
            Name = name;
            Affiliation = affiliation;
            Categories = categories.AsReadOnly();
        }

        public static implicit operator Role(string name) => Roles[name];

        private static Dictionary<string, Role> LoadAll()
        {
            var roles = new Dictionary<string, Role>();
            var yamlRoles = Resource.FromDirectory("Roles", "*.yml");

            foreach (YamlMappingNode yaml in yamlRoles)
            {
                var name = yaml["name"].AsString();
                Team affiliation = yaml["affiliation"].AsString();
                var categories = new List<string>();
                YamlSequenceNode yamlCategories = (YamlSequenceNode)yaml["categories"];

                foreach (var category in yamlCategories)
                {
                    categories.Add(category.AsString());
                }

                var role = new Role(name, affiliation, categories);
                roles.Add(name, role);
            }

            return roles;
        }
    }
}
