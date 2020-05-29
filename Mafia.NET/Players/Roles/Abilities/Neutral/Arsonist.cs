using System;
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

        public void UnDouse()
        {
            User.Doused = false;
            var notification = Notification.Chat(ArsonistKey.UnDouse);
            User.OnNotification(notification);
        }

        public void Douse(IPlayer target)
        {
            target.Doused = true;

            if (Setup.VictimNoticesDousing || target.Role.Ability is Arsonist)
            {
                var notification = target.Role.Ability is Arsonist
                    ? Notification.Chat(ArsonistKey.ArsonistDouse)
                    : Notification.Chat(ArsonistKey.OtherDouse);
                    
                target.OnNotification(notification);
            }
        }

        public void Ignite()
        {
            foreach (var player in Match.LivingPlayers)
            {
                if (!player.Doused) continue;

                var stoppable = !Setup.IgnitionAlwaysKills;
                PiercingAttack(player, false, stoppable);

                if (Setup.IgnitionKillsVictimsTargets &&
                    player.TargetManager.Try(out var victimsTarget))
                    PiercingAttack(victimsTarget, false, stoppable);
            }
        }
        
        public override void Try(Action<IAbilityAction> action)
        {
            action(this); 
        }

        public override void Kill()
        {
            var target = TargetManager[0];

            if (target == null && Active) UnDouse();
            else if (target == User && Active) Ignite();
            else if (target != null) Douse(target);
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

    public class ArsonistSetup : INightImmune
    {
        public bool IgnitionKillsVictimsTargets = true;
        public bool IgnitionAlwaysKills = true;
        public bool NightImmune { get; set; } = true;
        public bool VictimNoticesDousing = true;
        public bool DousesRoleBlockers = true;
    }
}