using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players
{
    class Player : IPlayer
    {
        public string Name { get; set; }
        public IRole Role { get; set; }

        public Player(string name, IRole role)
        {
            Name = name;
            Role = role;
        }
    }
}
