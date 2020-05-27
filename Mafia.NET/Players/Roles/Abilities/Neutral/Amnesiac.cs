using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterAbility("Amnesiac", typeof(AmnesiacSetup))]
    public class Amnesiac : BaseAbility<AmnesiacSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return true;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.NotSuspicious;
        }
    }

    public class AmnesiacSetup : IAbilitySetup
    {
    }
}