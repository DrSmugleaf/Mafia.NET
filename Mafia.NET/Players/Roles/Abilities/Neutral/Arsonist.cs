using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterKey]
    public enum ArsonistKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        ArsonistDouse,
        OtherDouse,
        UnDouse,
        Ignite,
        DouseTarget
    }

    [RegisterAbility("Arsonist", typeof(ArsonistSetup))]
    public class Arsonist : BaseAbility<ArsonistSetup>
    {
        public Notification TargetAddMessage(IPlayer target)
        {
            return target == User
                ? Notification.Chat(ArsonistKey.Ignite)
                : Notification.Chat(ArsonistKey.UserAddMessage, target);
        }

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var douse = new Douse(this);
            actions.Add(douse);

            var ignite = new Ignite(this);
            actions.Add(ignite);
        }

        public override bool BlockedBy(IPlayer blocker)
        {
            if (Setup.DousesRoleBlockers) TargetManager.ForceSet(blocker);
            return base.BlockedBy(blocker);
        }

        public override bool PiercingBlockedBy(IPlayer blocker)
        {
            if (Setup.DousesRoleBlockers) TargetManager.ForceSet(blocker);
            return base.PiercingBlockedBy(blocker);
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Living(Match);
            AddTarget(filter, TargetNotification.Enum<ArsonistKey>());
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsArsonist;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.Arsonist;
        }
    }

    public class ArsonistSetup : INightImmune, IDouseSetup, IIgniteSetup
    {
        public bool DousesRoleBlockers { get; set; } = true;
        public bool IgnitionAlwaysKills { get; set; } = true;
        public bool IgnitionKillsVictimsTargets { get; set; } = true;
        public bool VictimNoticesDousing { get; set; } = true;
        public int NightImmunity { get; set; } = (int) AttackStrength.Base;
    }
}