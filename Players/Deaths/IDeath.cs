namespace Mafia.NET.Players.Deaths
{
    public interface IDeath
    {
        int Day { get; }
        IPlayer Of { get; }
        DeathCause Cause { get; }
        string LastWill { get; }
        string DeathNote { get; }
    }
}
