using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum EscortKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetRoleBlockImmune
    }

    [RegisterAbility("Escort", typeof(EscortSetup))]
    public class Escort : TownAbility<EscortSetup>
    {
        public override void Block()
        {
            if (!TargetManager.Try(out var target)) return;

            User.Crimes.Add(CrimeKey.Soliciting);

            if (target.Role.Team.Id == "Town")
                User.Crimes.Add(CrimeKey.DisturbingThePeace);

            if (!target.Role.Ability.BlockedBy(User) && Setup.DetectsBlockImmuneTarget)
            {
                var notification = Notification.Chat(EscortKey.TargetRoleBlockImmune);
                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<EscortKey>());
        }
    }

    public class EscortSetup : ITownSetup, IRoleBlockImmune
    {
        public bool DetectsBlockImmuneTarget = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}