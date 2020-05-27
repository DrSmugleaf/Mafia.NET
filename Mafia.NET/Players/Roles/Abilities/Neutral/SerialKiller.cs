using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterAbility("Serial Killer", typeof(SerialKillerSetup))]
    public class SerialKiller : BaseAbility<SerialKillerSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsSerialKiller;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.SerialKiller;
        }
    }

    public class SerialKillerSetup : IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}