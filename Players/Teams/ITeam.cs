using System.Drawing;

namespace Mafia.NET.Players.Teams
{
    interface ITeam
    {
        string Name { get; }
        Color Tint { get; }
    }
}
