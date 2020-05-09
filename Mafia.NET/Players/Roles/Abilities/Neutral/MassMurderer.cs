namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class MassMurderer : GuiltyAbility<MassMurdererSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsMassMurderer;
    }

    public class MassMurdererSetup : IAbilitySetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}
