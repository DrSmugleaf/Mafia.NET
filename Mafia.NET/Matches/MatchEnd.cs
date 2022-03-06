using System;

namespace Mafia.NET.Matches;

public class MatchEnd : EventArgs
{
    public MatchEnd(IMatch match)
    {
        Match = match;
    }

    public IMatch Match { get; }
}