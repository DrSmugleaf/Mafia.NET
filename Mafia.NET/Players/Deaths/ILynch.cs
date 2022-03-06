using System.Collections.Generic;

namespace Mafia.NET.Players.Deaths;

public interface ILynch : IDeath
{
    IList<IPlayer> For { get; }
    IList<IPlayer> Against { get; }
    IList<IPlayer> Abstained { get; }
}