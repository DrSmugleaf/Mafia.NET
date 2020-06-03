using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum RememberKey
    {
        RememberAnnouncement,
        RememberPersonal,
        StillAlive,
        Unable
    }

    public class Remember : AbilityAction<IRememberSetup>
    {
        public Remember(
            IAbility<IRememberSetup> ability,
            int priority = 10,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool TryUse()
        {
            if (!Filter(this))
            {
                var notification = Notification.Chat(Ability, RememberKey.Unable);
                User.OnNotification(notification);
            }

            return base.TryUse();
        }

        public override bool Use(IPlayer target)
        {
            if (target.Alive)
            {
                var notification = Notification.Chat(Ability, RememberKey.StillAlive);
                User.OnNotification(notification);

                return false;
            }
            else
            {
                if (Setup.NewRoleRevealedToTown)
                {
                    var announcement = Notification.Popup(Ability, RememberKey.RememberAnnouncement, target.Role);
                    Match.Graveyard.Announcements.Add(announcement);
                }

                var notification = Notification.Chat(Ability, RememberKey.RememberPersonal, target.Role);
                User.ChangeRole(target.Role);

                target.Role.Ability.User.OnNotification(notification);

                return true;
            }
        }
    }

    public interface IRememberSetup : IAbilitySetup
    {
        public bool CanBecomeKillingRole { get; set; }
        public bool CanBecomeMafiaTriad { get; set; }
        public bool CanBecomeTown { get; set; }
        public bool NewRoleRevealedToTown { get; set; }
        public bool ExcludedFromRandoms { get; set; }
    }
}