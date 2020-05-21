using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class WitchDoctor : GuiltyAbility<WitchDoctorSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsCult;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.Cultist;
        }
    }

    public class WitchDoctorSetup : IAbilitySetup
    {
    }
}