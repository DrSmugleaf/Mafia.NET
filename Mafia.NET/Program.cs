using System;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var teams = Team.Teams;
            var roles = RoleRegistry.Default;
            var categories = Category.Categories;
            var abilities = AbilityRegistry.Default;
            Console.ReadLine();
        }
    }
}