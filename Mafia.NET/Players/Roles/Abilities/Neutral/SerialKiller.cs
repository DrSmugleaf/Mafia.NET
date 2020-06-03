using System.Collections.Generic;
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
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var attack = new SerialKillerAttack(this, AttackStrength.Base);
            actions.Add(attack);
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

    public class SerialKillerSetup : ISerialKillerSetup
    {
        public bool KillsRoleBlockers { get; set; }
        public bool WinsTiesOverArsonist { get; set; } = false;
        public bool DetectionImmune { get; set; }
        public int NightImmunity { get; set; } = (int) AttackStrength.Base;
    }
}