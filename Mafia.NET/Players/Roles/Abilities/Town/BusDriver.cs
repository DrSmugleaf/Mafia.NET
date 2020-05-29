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
    public class BusDriver : TownAbility<BusDriverSetup> // TODO Murder crime
    {
        public override void Switch()
        {
            if (TargetManager.Try(out var first) && TargetManager.Try(1, out var second))
            {
                var notification1 = SwitchMessage(first);
                first.OnNotification(notification1);

                var notification2 = SwitchMessage(second);
                second.OnNotification(notification2);

                foreach (var player in Match.LivingPlayers)
                {
                    var targets = player.TargetManager.Get().Targets;
                    for (var i = 0; i < targets.Count; i++)
                    {
                        var target = targets[i];
                        if (target.Targeted == first) target.ForceSet(second);
                        else if (target.Targeted == second) target.ForceSet(first);
                    }
                }
            }
        }

        public Notification SwitchMessage(IPlayer target)
        {
            return target == User
                ? Notification.Chat(BusDriverKey.SelfRide)
                : Notification.Chat(BusDriverKey.OtherRide);
        }

        public Notification TargetMessage()
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
                UserAddMessage = target => TargetMessage(),
                UserRemoveMessage = target => TargetMessage(),
                UserChangeMessage = (old, current) => TargetMessage()
            });

            AddTarget(filter, new TargetNotification
            {
                UserAddMessage = target => TargetMessage(),
                UserRemoveMessage = target => TargetMessage(),
                UserChangeMessage = (old, current) => TargetMessage()
            });
        }
    }

    public class BusDriverSetup : ITownSetup
    {
        public bool CanTargetSelf = false;
    }
}