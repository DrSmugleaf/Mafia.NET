using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum ConsortKey
    {
        CantRoleBlock,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Consort", typeof(ConsortSetup))]
    public class Consort : MafiaAbility<ConsortSetup>, IRoleBlocker
    {
        public void Block(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Soliciting);
            if (target.Role.Team.Id == "Town") User.Crimes.Add(CrimeKey.DisturbingThePeace);

            target.Role.Ability.Disable();
            if (target.Role.Ability.Active && Setup.DetectsBlockImmunity)
            {
                var message = Notification.Chat(ConsortKey.CantRoleBlock);
                User.OnNotification(message);
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), TargetNotification.Enum<ConsortKey>());
        }
    }

    public class ConsortSetup : MafiaMinionSetup, IRoleBlockImmune
    {
        public bool DetectsBlockImmunity = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}