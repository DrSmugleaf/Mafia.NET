using Mafia.NET.Player.Roles;

namespace Mafia.NET.Player
{
    interface IPlayer
    {
        string Name { get; set; }
        IRole Role { get; set; }
    }
}
