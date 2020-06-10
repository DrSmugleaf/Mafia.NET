using System.Collections.Generic;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities.Bases
{
    public abstract class NightEndAbility : Ability
    {
        public override void NightEnd(in IList<IAbility> abilities)
        {
            abilities.Add(this);
        }
    }

    public abstract class NightEndAbility<T> : Ability<T> where T : IAbilitySetup
    {
        public override void NightEnd(in IList<IAbility> abilities)
        {
            abilities.Add(this);
        }
    }
}