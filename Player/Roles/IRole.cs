using Mafia.NET.Player.Roles.Categories;
using Mafia.NET.Player.Teams;
using System.Collections.Generic;

namespace Mafia.NET.Player.Roles
{
    interface IRole
    {
        string Name { get; }
        ITeam Affiliation { get; }
        IReadOnlyList<ICategory> Categories { get; }
    }
}
