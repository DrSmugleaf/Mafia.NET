#nullable enable

namespace Mafia.NET.Players.Deaths
{
    public interface IDeath
    {
        int Day { get; set; }
        IPlayer Victim { get; set; }
        DeathCause Cause { get; set; }
        IPlayer? Killer { get; set; }
        Note LastWill { get; set; }
        Note? DeathNote { get; set; }
    }
}
