namespace Mafia.NET.Players.Deaths
{
    public class Death : IDeath
    {
        public int Day { get; }
        public IPlayer Of { get; }
        public DeathCause Cause { get; }
        public string LastWill { get; }
        public string DeathNote { get; }

        public Death(int day, IPlayer of, DeathCause cause, string lastWill, string deathNote)
        {
            Day = day;
            Of = of;
            Cause = cause;
            LastWill = lastWill;
            DeathNote = deathNote;
        }
    }
}
