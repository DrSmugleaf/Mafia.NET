using Mafia.NET.Extension;
using Mafia.NET.Resources;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Categories
{
    class Category : ICategory
    {
        public static readonly IReadOnlyDictionary<string, Category> Categories = LoadAll();
        public string Name { get; }
        public string Description { get; }

        public Category(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static explicit operator Category(string name) => Categories[name];

        private static Dictionary<string, Category> LoadAll()
        {
            var categories = new Dictionary<string, Category>();
            var yamlCategories = Resource.FromDirectory("Categories", "*.yml");

            foreach (YamlMappingNode yaml in yamlCategories)
            {
                var name = yaml["name"].AsString();
                var description = yaml["description"].AsString();
                var category = new Category(name, description);
                categories.Add(name, category);
            }

            return categories;
        }
    }
}
