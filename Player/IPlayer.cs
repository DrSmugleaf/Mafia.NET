using Mafia.NET.Player.Role;

namespace Mafia.NET.Player
{
    interface IPlayer
    {
        string Name { get; set; }
        IRole Role { get; set; }
    }
}
