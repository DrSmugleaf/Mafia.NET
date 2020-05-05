#nullable enable

namespace Mafia.NET.Players.Deaths
{
    public class Death : IDeath
    {
        public int Day { get; set; }
        public IPlayer Victim { get; set; }
        public DeathCause Cause { get; set; }
        public IPlayer? Killer { get; set; }
        public string LastWill { get; set; }
        public string? DeathNote { get; set; }

        public Death(int day, IPlayer victim, DeathCause cause, IPlayer? killer = null)
        {
            Day = day;
            Victim = victim;
            Cause = cause;
            Killer = killer;
            LastWill = victim.LastWill;
            DeathNote = killer?.DeathNote;
        }
    }
}
