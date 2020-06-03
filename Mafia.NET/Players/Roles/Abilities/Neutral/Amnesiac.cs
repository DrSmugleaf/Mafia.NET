using System.Collections.Generic;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;
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
        public bool Compatible([CanBeNull] IPlayer target)
        {
            if (target == null) return false;

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

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var remember = new Remember(this) {Filter = action => Compatible(action.TargetManager[0])};
            actions.Add(remember);
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

    public class AmnesiacSetup : IRememberSetup, IRandomExcluded
    {
        public bool CanBecomeKillingRole { get; set; } = true;
        public bool CanBecomeMafiaTriad { get; set; } = true;
        public bool CanBecomeTown { get; set; } = true;
        public bool NewRoleRevealedToTown { get; set; } = true;
        public bool ExcludedFromRandoms { get; set; } = false;
    }
}