using System.Collections.Generic;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum RememberKey
    {
        RememberAnnouncement,
        RememberPersonal,
        StillAlive,
        Unable
    }

    [RegisterAbility("Remember", 10, typeof(RememberSetup))]
    public class Remember : NightEndAbility<RememberSetup>
    {
        // TODO: Role-block and mind-control immunity, minimum of 1 charge on role change
        public bool Compatible([CanBeNull] IPlayer target)
        {
            if (target == null) return false;

            var teamId = target.Role.Team.Id;
            var role = target.Role;

            if (!Setup.CanBecomeTown && teamId == "Town") return false;
            if (!Setup.CanBecomeMafiaTriad &&
                (teamId == "Mafia" || teamId == "Triad")) return false;
            if (!Setup.CanBecomeKillingRole && (
                role.IsCategory("Town Killing") ||
                role.IsCategory("Mafia Killing") ||
                role.IsCategory("Triad Killing") ||
                role.IsCategory("Neutral Killing"))) return false;
            if (role.Unique) return false;
            return true;
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Dead(Match).Where(Compatible);
            SetupTargets<RememberKey>(abilities, filter);
        }

        public override bool CanUse(IPlayer target)
        {
            return base.CanUse(target) && Compatible(target);
        }

        public override bool TryUse(IPlayer target)
        {
            if (!Filter(this))
            {
                var notification = Notification.Chat(Role, RememberKey.Unable);
                User.OnNotification(notification);
            }

            return base.TryUse(target);
        }

        public override bool Use(IPlayer target)
        {
            if (target.Alive)
            {
                var notification = Notification.Chat(Role, RememberKey.StillAlive);
                User.OnNotification(notification);

                return false;
            }

            if (Compatible(target))
            {
                if (Setup.NewRoleRevealedToTown)
                {
                    var announcement = Notification.Popup(Role, RememberKey.RememberAnnouncement, target.Role);
                    Match.Graveyard.Announcements.Add(announcement);
                }

                var notification = Notification.Chat(Role, RememberKey.RememberPersonal, target.Role);
                User.ChangeRole(target.Role);
                User.OnNotification(notification);

                return true;
            }

            return false;
        }
    }

    public class RememberSetup : IAbilitySetup
    {
        public bool CanBecomeKillingRole { get; set; } = true;
        public bool CanBecomeMafiaTriad { get; set; } = true;
        public bool CanBecomeTown { get; set; } = true;
        public bool NewRoleRevealedToTown { get; set; } = true;
    }
}