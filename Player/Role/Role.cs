using Mafia.NET.Player.Team;

namespace Mafia.NET.Player.Role
{
    abstract class Role : IRole
    {
        public string Name { get; }
        public ITeam Team { get; }

        public Role(string name, ITeam team)
        {
            Name = name;
            Team = team;
        }
    }
}
