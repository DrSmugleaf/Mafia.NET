using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Deaths;

public interface IDeath
{
    int Day { get; set; }
    IPlayer Victim { get; }
    Text VictimName { get; set; }
    Key? VictimRole { get; set; } // TODO
    DeathCause Cause { get; set; }
    IPlayer? Killer { get; set; }
    string? LastWill { get; set; }
    string? DeathNote { get; set; }
    string Description { get; set; }
    AttackStrength Strength { get; set; }
    bool Direct { get; set; }
    bool Stoppable { get; set; }

    void WithVictim(IPlayer player);
}