using Mafia.NET.Player.Team;

namespace Mafia.NET.Player.Role
{
    class Role : IRole
    {
        public string Name { get; set; }
        public ITeam Team { get; set; }

        public Role(string name, ITeam team)
        {
            Name = name;
            Team = team;
        }
    }
}
