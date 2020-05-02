using Mafia.NET.Matches.Options.DayTypes;

namespace Mafia.NET.Matches.Options
{
    public class Settings : ISettings
    {
        public IDayType DayType { get; }

        public Settings(IDayType dayType)
        {
            DayType = dayType;
        }
    }
}
