﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles
{
    public interface IRole
    {
        string Name { get; }
        string Summary { get; }
        string Goal { get; }
        string Abilities { get; }
        ITeam Affiliation { get; }
        IReadOnlyList<ICategory> Categories { get; }
        Color Tint { get; }
        IAbility Ability { get; set; }

        bool IsCategory(string name);
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
            Affiliation = role.Affiliation;
            Categories = role.Categories;
            Tint = role.Tint;
            Ability = ability;
        }

        public string Name { get; }
        public string Summary { get; }
        public string Goal { get; }
        public string Abilities { get; }
        public ITeam Affiliation { get; }
        public IReadOnlyList<ICategory> Categories { get; }
        public Color Tint { get; }
        public IAbility Ability { get; set; }

        public bool IsCategory(string name)
        {
            return Categories.Any(category => category.Name == name);
        }

        public IReadOnlyList<Goal> Goals()
        {
            return Categories.Select(category => category.Goal).ToList();
        }

        public IReadOnlyList<Goal> Enemies()
        {
            return Categories.SelectMany(category => category.Goal.Enemies()).ToList();
        }
    }
}