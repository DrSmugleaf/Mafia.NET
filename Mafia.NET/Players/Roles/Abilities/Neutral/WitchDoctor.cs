namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class WitchDoctor : GuiltyAbility<WitchDoctorSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsCult;
        }

        protected override string GuiltyName()
        {
            return "Cultist";
        }
    }

    public class WitchDoctorSetup : IAbilitySetup
    {
    }
}