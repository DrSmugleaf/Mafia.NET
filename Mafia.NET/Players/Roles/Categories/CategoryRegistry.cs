using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Registries;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Categories;

public class CategoryRegistry : ImmutableRegistry<ICategory>
{
    private static readonly Lazy<CategoryRegistry> Lazy = new(() => new CategoryRegistry());

    public CategoryRegistry(Dictionary<string, ICategory> ids) : base(ids)
    {
    }

    private CategoryRegistry() : this(LoadAll())
    {
    }

    public static CategoryRegistry Default => Lazy.Value;

    public static List<YamlMappingNode> LoadYaml()
    {
        var categories = Resource.FromDirectory("Categories", "*.yml");
        return categories.Select(category => (YamlMappingNode) category).ToList();
    }

    public static Dictionary<string, ICategory> LoadAll()
    {
        var categories = new Dictionary<string, ICategory>();

        foreach (var yaml in LoadYaml())
        {
            var id = yaml["id"].AsString();
            var goal = yaml["goal"].AsEnum<Goal>();
            var team = yaml["team"].AsString();
            var category = new Category(id, goal, team);

            categories.Add(id, category);
        }

        return categories;
    }
}