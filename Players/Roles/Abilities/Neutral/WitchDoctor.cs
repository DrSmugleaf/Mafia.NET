namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class WitchDoctor : GuiltyAbility<WitchDoctorSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsCult;

        protected override string GuiltyName() => "Cultist";
    }

    public class WitchDoctorSetup : IAbilitySetup
    {

    }
}
