using System.Collections.Generic;
using Mafia.NET.Extension;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Categories
{
    public interface ICategory
    {
        string Name { get; }
        string Description { get; }
        Goal Goal { get; }
    }

    public class Category : ICategory
    {
        public static readonly IReadOnlyDictionary<string, Category> Categories = LoadAll();

        public Category(string name, string description, Goal goal)
        {
            Name = name;
            Description = description;
            Goal = goal;
        }

        public string Name { get; }
        public string Description { get; }
        public Goal Goal { get; }

        public static explicit operator Category(string name)
        {
            return Categories[name];
        }

        private static Dictionary<string, Category> LoadAll()
        {
            var categories = new Dictionary<string, Category>();
            var yamlCategories = Resource.FromDirectory("Categories", "*.yml");

            foreach (YamlMappingNode yaml in yamlCategories)
            {
                var name = yaml["name"].AsString();
                var description = yaml["description"].AsString();
                var goal = yaml["goal"].AsEnum<Goal>();
                var category = new Category(name, description, goal);
                categories.Add(name, category);
            }

            return categories;
        }
    }
}