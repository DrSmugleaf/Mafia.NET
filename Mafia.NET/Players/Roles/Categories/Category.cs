using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Categories;

public interface ICategory : ILocalizable, IRegistrable
{
    Key Name { get; }
    Key Description { get; }
    Goal Goal { get; }
    string Team { get; }
}

public class Category : ICategory
{
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

    public Text Localize(CultureInfo? culture = null)
    {
        return Name.Localize(culture);
    }

    public override bool Equals(object? obj)
    {
        return obj is ICategory o && Id.Equals(o.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return Localize().ToString();
    }
}