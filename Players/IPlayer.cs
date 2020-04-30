using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players
{
    interface IPlayer
    {
        string Name { get; set; }
        IRole Role { get; set; }
    }
}
