﻿using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Bus Driver", typeof(BusDriverSetup))]
    public class BusDriver : TownAbility<BusDriverSetup>, ISwitcher // TODO Murder crime
    {
        public Notification SwitchNotification(IPlayer target)
        {
            return target == User ?
                Notification.Chat("You gave yourself a ride to someplace else. You were switched!") :
                Notification.Chat("Someone gave you a ride to someplace else. You were switched!");
        }

        public string GetMessage()
        {
            var target1 = TargetManager[0];
            var target2 = TargetManager[0];

            if (target1 == null && target2 == null) return "You won't pick up anyone.";
            else if (target1 != null && target2 != null) return $"You will swap {target1.Name} with {target2.Name}";
            else if (target1 != null) return $"You will pick up {target1.Name}.";
            else return $"You will pick up {target2.Name}";
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf) filter = filter.Except(User);

            AddTarget(filter, new TargetNotification()
            {
                UserAddMessage = (target) => GetMessage(),
                UserRemoveMessage = (target) => GetMessage(),
                UserChangeMessage = (old, _new) => GetMessage()
            });

            AddTarget(filter, new TargetNotification()
            {
                UserAddMessage = (target) => GetMessage(),
                UserRemoveMessage = (target) => GetMessage(),
                UserChangeMessage = (old, _new) => GetMessage()
            });
        }

        public void Switch()
        {
            if (TargetManager.Try(out var target1) && TargetManager.Try(out var target2))
            {
                var notification1 = SwitchNotification(target1);
                target1.OnNotification(notification1);

                var notification2 = SwitchNotification(target2);
                target2.OnNotification(notification2);

                foreach (var player in Match.LivingPlayers.Values)
                {
                    var targets = player.Role.Ability.TargetManager.Get().Targets;
                    for (var i = 0; i < targets.Count; i++)
                    {
                        var target = targets[i];
                        if (target.Targeted == target1) target.ForceSet(target2);
                        else if (target.Targeted == target2) target.ForceSet(target1);
                    }
                }
            }
        }
    }

    public class BusDriverSetup : ITownSetup
    {
        public bool CanTargetSelf = false;
    }
}