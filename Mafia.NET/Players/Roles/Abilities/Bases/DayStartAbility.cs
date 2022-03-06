using System.Collections.Generic;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities.Bases;

public abstract class DayStartAbility : Ability
{
    public override void DayStart(in IList<IAbility> abilities)
    {
        abilities.Add(this);
    }
}

public abstract class DayStartAbility<T> : Ability<T> where T : IAbilitySetup
{
    public override void DayStart(in IList<IAbility> abilities)
    {
        abilities.Add(this);
    }
}