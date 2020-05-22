using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum BusDriverKey
    {
        NoTargets,
        OneTarget,
        TwoTargets,
        SelfRide,
        OtherRide
    }

    [RegisterAbility("Bus Driver", typeof(BusDriverSetup))]
    public class BusDriver : TownAbility<BusDriverSetup>, ISwitcher // TODO Murder crime
    {
        public void Switch()
        {
            if (TargetManager.Try(out var first) && TargetManager.Try(1, out var second))
            {
                var notification1 = SwitchNotification(first);
                first.OnNotification(notification1);

                var notification2 = SwitchNotification(second);
                second.OnNotification(notification2);

                foreach (var player in Match.LivingPlayers)
                {
                    var targets = player.Role.Ability.TargetManager.Get().Targets;
                    for (var i = 0; i < targets.Count; i++)
                    {
                        var target = targets[i];
                        if (target.Targeted == first) target.ForceSet(second);
                        else if (target.Targeted == second) target.ForceSet(first);
                    }
                }
            }
        }

        public Notification SwitchNotification(IPlayer target)
        {
            return target == User
                ? Notification.Chat(BusDriverKey.SelfRide)
                : Notification.Chat(BusDriverKey.OtherRide);
        }

        public Notification GetMessage()
        {
            var target1 = TargetManager[0];
            var target2 = TargetManager[0];

            if (target1 == null && target2 == null) return Notification.Chat(BusDriverKey.NoTargets);
            if (target1 != null && target2 != null) return Notification.Chat(BusDriverKey.TwoTargets, target1, target2);
            if (target1 != null) return Notification.Chat(BusDriverKey.OneTarget, target1);
            return Notification.Chat(BusDriverKey.OneTarget, target2);
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf) filter = filter.Except(User); // TODO Except already targeted

            AddTarget(filter, new TargetNotification
            {
                UserAddMessage = target => GetMessage(),
                UserRemoveMessage = target => GetMessage(),
                UserChangeMessage = (old, current) => GetMessage()
            });

            AddTarget(filter, new TargetNotification
            {
                UserAddMessage = target => GetMessage(),
                UserRemoveMessage = target => GetMessage(),
                UserChangeMessage = (old, current) => GetMessage()
            });
        }
    }

    public class BusDriverSetup : ITownSetup
    {
        public bool CanTargetSelf = false;
    }
}