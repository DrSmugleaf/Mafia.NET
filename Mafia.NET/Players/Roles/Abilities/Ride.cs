using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum RideKey
    {
        NoTargets,
        OneTarget,
        TwoTargets,
        SelfRide,
        OtherRide
    }

    [RegisterAbility("Ride", 2, typeof(RideSetup))]
    public class Ride : NightEndAbility<RideSetup>
    {
        public Notification UserMessage()
        {
            var first = Targets[0];
            var second = Targets[0];

            if (first == null && second == null) return Notification.Chat(RideKey.NoTargets);
            if (first != null && second != null) return Notification.Chat(RideKey.TwoTargets, first, second);
            if (first != null) return Notification.Chat(RideKey.OneTarget, first);
            return Notification.Chat(RideKey.OneTarget, second);
        }

        public Notification SwitchMessage(IPlayer target)
        {
            return target == User
                ? Notification.Chat(Role, RideKey.SelfRide)
                : Notification.Chat(Role, RideKey.OtherRide);
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf) filter = filter.Except(User); // TODO Except already targeted

            var notification = new TargetNotification
            {
                UserAddMessage = target => UserMessage(),
                UserRemoveMessage = target => UserMessage(),
                UserChangeMessage = (old, current) => UserMessage()
            };

            SetupTargets(abilities, filter, notification);
            SetupTargets(abilities, filter, notification);
        }

        public override bool Use(IPlayer first, IPlayer second)
        {
            var firstNotification = SwitchMessage(first);
            first.OnNotification(firstNotification);

            var secondNotification = SwitchMessage(second);
            second.OnNotification(secondNotification);

            foreach (var player in Match.LivingPlayers)
            {
                var targets = player.Targets.Get().Targets;
                foreach (var target in targets)
                    if (target.Targeted == first) target.ForceSet(second);
                    else if (target.Targeted == second) target.ForceSet(first);
            }

            return true;
        }
    }

    public class RideSetup : IAbilitySetup
    {
        public bool CanTargetSelf { get; set; } = false;
    }
}