namespace Mafia.NET.Players.Deaths
{
#nullable enable
    public interface IDeath
    {
        int Day { get; set; }
        IPlayer Victim { get; }
        string VictimName { get; set; }
        string? VictimRole { get; set; }
        DeathCause Cause { get; set; }
        IPlayer? Killer { get; set; }
        string LastWill { get; set; }
        string? DeathNote { get; set; }
        string Description { get; set; }

        IDeath WithVictim(IPlayer player);
    }
}