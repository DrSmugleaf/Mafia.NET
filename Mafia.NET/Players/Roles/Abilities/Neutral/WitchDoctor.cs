using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterAbility("Witch Doctor", typeof(WitchDoctorSetup))]
    public class WitchDoctor : BaseAbility<WitchDoctorSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup = null)
        {
            return setup?.DetectsCult == true;
        }

        public override Key GuiltyName()
        {
            return SheriffKey.Cultist;
        }
    }

    public class WitchDoctorSetup : IAbilitySetup
    {
    }
}