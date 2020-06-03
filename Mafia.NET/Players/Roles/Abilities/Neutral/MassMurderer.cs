using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterAbility("Mass Murderer", typeof(MassMurdererSetup))]
    public class MassMurderer : BaseAbility<MassMurdererSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsMassMurderer;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.MassMurderer;
        }
    }

    public class MassMurdererSetup : IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}