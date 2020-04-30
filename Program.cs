using Mafia.NET.Player.Roles;
using Mafia.NET.Player.Teams;
using System;

namespace Mafia.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            var teams = Team.Teams;
            var roles = Role.LoadAll();
            Console.WriteLine();
        }
    }
}
