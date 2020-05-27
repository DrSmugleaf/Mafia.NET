using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterAbility("Arsonist", typeof(ArsonistSetup))]
    public class Arsonist : GuiltyAbility<ArsonistSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsArsonist;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.Arsonist;
        }
    }

    public class ArsonistSetup : IAbilitySetup
    {
    }
}