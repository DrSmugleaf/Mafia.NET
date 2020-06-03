using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum HealKey
    {
        TargetAttacked
    }

    public class Heal : AbilityAction<IHealSetup>
    {
        public Heal(IAbility<IHealSetup> user, int priority = 7) : base(user, priority)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (target.Role.Ability.HealedBy(User) && Setup.KnowsIfTargetAttacked)
            {
                var notification = Notification.Chat(Ability, HealKey.TargetAttacked);
                User.OnNotification(notification);
            }

            return true;
        }
    }

    public interface IHealSetup : IAbilitySetup
    {
        public bool KnowsIfTargetAttacked { get; set; }
    }
}