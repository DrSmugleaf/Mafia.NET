using System.Drawing;

namespace Mafia.NET.Players.Teams
{
    public interface ITeam
    {
        string Name { get; }
        Color Tint { get; }
    }
}
