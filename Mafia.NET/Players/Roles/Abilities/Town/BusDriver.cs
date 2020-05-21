using Mafia.NET.Localization;

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

    [RegisterAbility("Bus Driver", typeof(BusDriverSetup))] // TODO: Localize role names
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

        public Entry SwitchNotification(IPlayer target)
        {
            return target == User
                ? Entry.Chat(BusDriverKey.SelfRide)
                : Entry.Chat(BusDriverKey.OtherRide);
        }

        public Entry GetMessage()
        {
            var target1 = TargetManager[0];
            var target2 = TargetManager[0];

            if (target1 == null && target2 == null) return Entry.Chat(BusDriverKey.NoTargets);
            if (target1 != null && target2 != null) return Entry.Chat(BusDriverKey.TwoTargets, target1, target2);
            if (target1 != null) return Entry.Chat(BusDriverKey.OneTarget, target1);
            return Entry.Chat(BusDriverKey.OneTarget, target2);
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