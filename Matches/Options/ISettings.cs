using Mafia.NET.Matches.Phases;

namespace Mafia.NET.Matches.Options
{
    public interface ISettings
    {
        IPhase Procedure { get; }
        bool AnonymousVoting { get; }
    }
}
