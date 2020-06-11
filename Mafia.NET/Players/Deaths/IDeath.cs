using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Deaths
{
    public interface IDeath
    {
        int Day { get; set; }
        IPlayer Victim { get; }
        Text VictimName { get; set; }
        [CanBeNull] Key VictimRole { get; set; } // TODO
        DeathCause Cause { get; set; }
        [CanBeNull] IPlayer Killer { get; set; }
        [CanBeNull] string LastWill { get; set; }
        [CanBeNull] string DeathNote { get; set; }
        string Description { get; set; }
        AttackStrength Strength { get; set; }
        bool Direct { get; set; }
        bool Stoppable { get; set; }

        void WithVictim(IPlayer player);
    }
}