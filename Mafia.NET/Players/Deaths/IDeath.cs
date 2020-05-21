using JetBrains.Annotations;

namespace Mafia.NET.Players.Deaths
{
    public interface IDeath
    {
        int Day { get; set; }
        IPlayer Victim { get; }
        string VictimName { get; set; }
        [CanBeNull] string VictimRole { get; set; } // TODO
        DeathCause Cause { get; set; }
        [CanBeNull] IPlayer Killer { get; set; }
        string LastWill { get; set; }
        [CanBeNull] string DeathNote { get; set; }
        string Description { get; set; }

        IDeath WithVictim(IPlayer player);
    }
}