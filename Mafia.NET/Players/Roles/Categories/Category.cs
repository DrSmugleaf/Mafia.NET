using System.Collections.Generic;
using System.Globalization;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Categories
{
    public interface ICategory : ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        Key Description { get; }
        Goal Goal { get; }
    }

    public class Category : ICategory
    {
        public static readonly IReadOnlyDictionary<string, Category> Categories = LoadAll();

        public Category(string id, Goal goal)
        {
            Id = id;
            Name = new Key($"{id}name");
            Description = new Key($"{id}description");
            Goal = goal;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Description { get; }
        public Goal Goal { get; }

        public static explicit operator Category(string id)
        {
            return Categories[id];
        }

        private static Dictionary<string, Category> LoadAll()
        {
            var categories = new Dictionary<string, Category>();
            var yamlCategories = Resource.FromDirectory("Categories", "*.yml");

            foreach (YamlMappingNode yaml in yamlCategories)
            {
                var id = yaml["id"].AsString();
                var goal = yaml["goal"].AsEnum<Goal>();
                var category = new Category(id, goal);
                categories.Add(id, category);
            }

            return categories;
        }

        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize();
        }
    }
}