namespace Mafia.NET.Players.Deaths
{
    public interface IDeath
    {
        int Day { get; set; }
        IPlayer Victim { get; set; }
        DeathCause Cause { get; set; }
        IPlayer Killer { get; set; }
        string LastWill { get; set; }
        string DeathNote { get; set; }
    }
}
