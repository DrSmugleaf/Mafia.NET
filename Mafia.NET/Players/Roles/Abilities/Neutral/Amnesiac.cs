using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    [RegisterKey]
    public enum AmnesiacKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        RememberAnnouncement,
        RememberPersonal,
        StillAlive,
        Unable
    }

    [RegisterAbility("Amnesiac", typeof(AmnesiacSetup))]
    public class Amnesiac : BaseAbility<AmnesiacSetup>
    {
        public bool Compatible(IPlayer target)
        {
            var teamId = target.Role.Team.Id;
            var role = target.Role;

            if (!Setup.CanBecomeTown && teamId == "Town") return false;
            if (!Setup.CanBecomeMafiaTriad &&
                (target.Role.Team.Id == "Mafia" || teamId == "Triad")) return false;
            if (!Setup.CanBecomeKillingRole && (
                role.IsCategory("Town Killing") ||
                role.IsCategory("Mafia Killing") ||
                role.IsCategory("Triad Killing") ||
                role.IsCategory("Neutral Killing"))) return false;
            if (role.Unique) return false;
            return true;
        }

        public override void Disguise()
        {
            if (!TargetManager.Try(out var target)) return;

            if (target.Alive)
            {
                var notification = Notification.Chat(AmnesiacKey.StillAlive);
                User.OnNotification(notification);
            }
            else if (Compatible(target))
            {
                if (Setup.NewRoleRevealedToTown)
                {
                    var announcement = Notification.Popup(AmnesiacKey.RememberAnnouncement, target.Role);
                    Match.Graveyard.Announcements.Add(announcement);
                }

                var notification = Notification.Chat(AmnesiacKey.RememberPersonal, target.Role);
                User.ChangeRole(target.Role);
                User.Role.Ability.User = User;
                User = null;

                target.Role.Ability.User.OnNotification(notification);
            }
            else
            {
                var notification = Notification.Chat(AmnesiacKey.Unable);
                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Dead(Match)
                .Where(player => !player.Role.Unique && Compatible(player));

            AddTarget(filter, TargetNotification.Enum<AmnesiacKey>());
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return true;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.NotSuspicious;
        }
    }

    public class AmnesiacSetup : IRandomExcluded
    {
        public bool CanBecomeKillingRole = true;
        public bool CanBecomeMafiaTriad = true;
        public bool CanBecomeTown = true;
        public bool NewRoleRevealedToTown = true;
        public bool ExcludedFromRandoms { get; set; } = false;
    }
}