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
        string Team { get; }

        List<RoleEntry> Roles(RoleRegistry registry);
    }

    public class Category : ICategory
    {
        public static readonly IReadOnlyDictionary<string, Category> Categories = LoadAll();

        public Category(string id, Goal goal, string team)
        {
            Id = id;
            Name = new Key($"{id}name");
            Description = new Key($"{id}description");
            Goal = goal;
            Team = team;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Description { get; }
        public Goal Goal { get; }
        public string Team { get; }

        public List<RoleEntry> Roles(RoleRegistry registry)
        {
            return registry.Category(this);
        }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override bool Equals(object obj)
        {
            return obj is ICategory o && Id.Equals(o.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

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
                var team = yaml["team"].AsString();
                var category = new Category(id, goal, team);

                categories.Add(id, category);
            }

            return categories;
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}