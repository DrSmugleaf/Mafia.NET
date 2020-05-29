using System;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterKey]
    public enum SerialKillerKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Serial Killer", typeof(SerialKillerSetup))]
    public class SerialKiller : BaseAbility<SerialKillerSetup>
    {
        public override void Try(Action<IAbilityAction> action)
        {
            action(this); 
        }

        public override void Kill()
        {
            if (!TargetManager.Try(out var target)) return;
            Attack(target);
        }

        public override bool BlockedBy(IPlayer blocker)
        {
            if (Setup.KillsRoleBlockers) TargetManager.ForceSet(blocker);
            return base.BlockedBy(blocker);
        }

        public override bool PiercingBlockedBy(IPlayer blocker)
        {
            if (Setup.KillsRoleBlockers) TargetManager.ForceSet(blocker);
            return base.PiercingBlockedBy(blocker);
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsSerialKiller;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.SerialKiller;
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<VeteranKey>());
        }
    }

    public class SerialKillerSetup : INightImmune, IDetectionImmune
    {
        public bool KillsRoleBlockers { get; set; } = false;
        public bool WinsTiesOverArsonist { get; set; } = false;
        public bool DetectionImmune { get; set; } = false;
        public bool NightImmune { get; set; } = true;
    }
}