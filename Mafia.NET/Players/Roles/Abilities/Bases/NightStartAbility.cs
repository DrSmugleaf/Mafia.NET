using System.Collections.Generic;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities.Bases;

public abstract class NightStartAbility : Ability
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        abilities.Add(this);
    }
}

public abstract class NightStartAbility<T> : Ability<T> where T : IAbilitySetup
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        abilities.Add(this);
    }
}