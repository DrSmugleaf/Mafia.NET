using Mafia.NET.Matches.Options.DayTypes;

namespace Mafia.NET.Matches.Options
{
    public class Settings : ISettings
    {
        public IVoting Voting { get; }

        public Settings(IVoting voting)
        {
            Voting = voting;
        }
    }
}
