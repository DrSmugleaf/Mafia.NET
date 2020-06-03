using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;

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
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var swap = new Ride(this);
            actions.Add(swap);
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