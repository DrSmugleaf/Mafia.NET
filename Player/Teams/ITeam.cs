using System.Drawing;

namespace Mafia.NET.Player.Teams
{
    interface ITeam
    {
        string Name { get; }
        Color Tint { get; }
    }
}
