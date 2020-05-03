using Mafia.NET.Matches.Phases;

namespace Mafia.NET.Matches.Options
{
    public class Settings : ISettings
    {
        public IPhase Procedure { get; }
        public bool AnonymousVoting { get; }

        public Settings(IPhase procedure, bool anonymousVoting)
        {
            Procedure = procedure;
            AnonymousVoting = anonymousVoting;
        }
    }
}
