using Mafia.NET.Extension;
using Mafia.NET.Player.Teams;
using Mafia.NET.Resources;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Player.Roles
{
    class Role : IRole
    {
        public string Name { get; }
        public ITeam Affiliation { get; }
        public IReadOnlyList<string> Categories { get; }

        public Role(string name, ITeam affiliation, List<string> categories)
        {
            Name = name;
            Affiliation = affiliation;
            Categories = categories.AsReadOnly();
        }

        public static List<Role> LoadAll()
        {
            var roles = new List<Role>();
            YamlSequenceNode townYaml = new Resource("roles/town.yml");

            foreach (var townEntry in townYaml)
            {
                var name = townEntry["name"].AsString();
                Team affiliation = townEntry["affiliation"].AsString();
                var categories = new List<string>();
                YamlSequenceNode yamlCategories = (YamlSequenceNode)townEntry["categories"];

                foreach (var category in yamlCategories)
                {
                    categories.Add(category.AsString());
                }

                var role = new Role(name, affiliation, categories);
                roles.Add(role);
            }

            return roles;
        }
    }
}
