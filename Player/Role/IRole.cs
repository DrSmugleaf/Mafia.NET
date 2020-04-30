using Mafia.NET.Player.Team;

namespace Mafia.NET.Player.Role
{
    interface IRole
    {
        string Name { get; }
        ITeam Team { get; }
    }
}
