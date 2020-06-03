using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum AutopsyKey
    {
        StillAlive,
        AutopsyRole,
        AutopsyLastWill
    }

    public class Autopsy : AbilityAction<IAutopsySetup>
    {
        public Autopsy(IAbility<IAutopsySetup> user, int priority = 9, bool direct = true, bool stoppable = true) :
            base(user, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (target.Alive)
            {
                var notification = Notification.Chat(Ability, AutopsyKey.StillAlive);
                User.OnNotification(notification);
                return false;
            }

            var message = new EntryBundle();
            message.Chat(Ability, AutopsyKey.AutopsyRole, target, target.Role);
            if (Setup.DiscoverLastWill && target.LastWill.Text.Length > 0)
                message.Chat(Ability, AutopsyKey.AutopsyLastWill, target.LastWill);

            User.OnNotification(message);

            return true;
        }
    }

    public interface IAutopsySetup : IAbilitySetup
    {
        public bool DiscoverAllTargets { get; set; } // TODO: Discover targets
        public bool DiscoverDeathType { get; set; } // TODO: Death type
        public bool DiscoverLastWill { get; set; }
    }
}