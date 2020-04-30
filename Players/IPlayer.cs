using Mafia.NET.Players.Roles;
using System.Drawing;

namespace Mafia.NET.Players
{
    interface IPlayer
    {
        int Id { get; }
        string Name { get; }
        IRole Role { get; set; }
        Color Tint { get; }
    }
}
