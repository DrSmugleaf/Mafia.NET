using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public enum RideKey
    {
        SelfRide,
        OtherRide
    }

    public class Ride : AbilityAction
    {
        public Ride(
            IAbility ability,
            int priority = 2,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
            Ability = ability;
        }

        public Notification SwitchMessage(IPlayer target)
        {
            return target == User
                ? Notification.Chat(Ability, RideKey.SelfRide)
                : Notification.Chat(Ability, RideKey.OtherRide);
        }

        public override bool Use(IPlayer first, IPlayer second)
        {
            var firstNotification = SwitchMessage(first);
            first.OnNotification(firstNotification);

            var secondNotification = SwitchMessage(second);
            second.OnNotification(secondNotification);

            foreach (var player in Match.LivingPlayers)
            {
                var targets = player.TargetManager.Get().Targets;
                foreach (var target in targets)
                    if (target.Targeted == first) target.ForceSet(second);
                    else if (target.Targeted == second) target.ForceSet(first);
            }

            return true;
        }
    }
}