using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public enum VestKey
    {
        UsedUpNow
    }

    public class Vest : SelfImmunity
    {
        public Vest(
            IAbility ability,
            int strength = 1,
            int priority = 1,
            bool direct = true,
            bool stoppable = true) :
            base(ability, strength, priority, direct, stoppable)
        {
        }

        public Vest(
            IAbility ability,
            AttackStrength strength = AttackStrength.Base,
            int priority = 1,
            bool direct = true,
            bool stoppable = true) :
            this(ability, (int) strength, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (Uses == 0 || !base.Use(target)) return false;
            Ability.Uses--;

            if (Uses == 0)
            {
                var notification = Notification.Chat(Ability, VestKey.UsedUpNow);
                User.OnNotification(notification);
            }

            return true;
        }
    }
}