using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum ShootKey
    {
        TargetImmune
    }

    public class Shoot : Attack
    {
        public Shoot(
            IAbility ability,
            int strength = 1,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            base(ability, strength, direct, stoppable, priority)
        {
        }

        public Shoot(
            IAbility ability,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            this(ability, (int) strength, direct, stoppable, priority)
        {
        }

        public override bool Use(IPlayer victim)
        {
            if (Uses == 0) return false;
            Ability.Uses--;

            if (base.Use(victim)) return true;

            var notification = Notification.Chat(Ability, ShootKey.TargetImmune);
            User.OnNotification(notification);

            return true;
        }
    }
}