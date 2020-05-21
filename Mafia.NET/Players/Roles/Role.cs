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
        Key Name { get; }
        string Summary { get; }
        string Goal { get; }
        string Abilities { get; }
        ITeam Team { get; }
        IReadOnlyList<ICategory> Categories { get; }
        IAbility Ability { get; set; }

        bool IsCategory(string id);
        IReadOnlyList<Goal> Goals();
        IReadOnlyList<Goal> Enemies();
    }

    public class Role : IRole
    {
        public Role(RoleEntry role, IAbility ability)
        {
            Name = role.Name;
            Summary = role.Summary;
            Goal = role.Goal;
            Abilities = role.Abilities;
            Team = role.Team;
            Categories = role.Categories;
            Color = role.Color;
            Ability = ability;
        }

        public Key Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public ITeam Team { get; }
        public IReadOnlyList<ICategory> Categories { get; }
        public Color Color { get; }
        public IAbility Ability { get; set; }

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

        public string Localize(CultureInfo culture)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Name.Localize();
        }
    }
}