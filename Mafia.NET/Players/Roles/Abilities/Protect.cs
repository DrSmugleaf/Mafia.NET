using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterAbility("Protect", 1)]
public class Protect : NightEndAbility
{
    public AttackStrength Strength { get; set; } = AttackStrength.Base;

    public override bool Use(IPlayer target)
    {
        target.Perks.CurrentDefense = Strength;
        return true;
    }
}