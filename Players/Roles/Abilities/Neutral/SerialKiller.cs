namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class SerialKiller : GuiltyAbility<SerialKillerSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsSerialKiller;
    }

    public class SerialKillerSetup : IAbilitySetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}
