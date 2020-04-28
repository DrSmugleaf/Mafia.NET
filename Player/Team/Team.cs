namespace Mafia.NET.Player.Team
{
    class Team : ITeam
    {
        public string Name { get; set; }

        public Team(string name)
        {
            Name = name;
        }
    }
}
