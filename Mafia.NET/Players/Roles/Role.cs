using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles
{
    public interface IRole : IColorizable, ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        Key Summary { get; }
        Key Goal { get; }
        Key Abilities { get; }
        ITeam Team { get; }
        IReadOnlyList<ICategory> Categories { get; }
        IAbility Ability { get; set; }
        bool Unique { get; }

        bool IsCategory(string id);
        IReadOnlyList<Goal> Goals();
        IReadOnlyList<Goal> Enemies();
    }

    public class Role : IRole
    {
        public Role(RoleEntry role, IAbility ability)
        {
            Id = role.Id;
            Name = role.Name;
            Summary = role.Summary;
            Goal = role.Goal;
            Abilities = role.Abilities;
            Team = role.Team;
            Categories = role.Categories;
            Color = role.Color;
            Ability = ability;
            Unique = role.Unique;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key Abilities { get; }
        public ITeam Team { get; }
        public IReadOnlyList<ICategory> Categories { get; }
        public Color Color { get; }
        public IAbility Ability { get; set; }
        public bool Unique { get; }

        public bool IsCategory(string id)
        {
            return Categories.Any(category => category.Id == id);
        }

        public IReadOnlyList<Goal> Goals()
        {
            return Categories.Select(category => category.Goal).ToList();
        }

        public IReadOnlyList<Goal> Enemies()
        {
            return Categories.SelectMany(category => category.Goal.Enemies()).ToList();
        }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}