using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Consort", typeof(ConsortSetup))]
    public class Consort : MafiaAbility<ConsortSetup>, IRoleBlocker
    {
        public void Block(IPlayer target)
        {
            User.Crimes.Add("Soliciting");
            if (target.Role.Team.Name == "Town") User.Crimes.Add("Disturbing the peace");

            target.Role.Ability.Disable();
            if (target.Role.Ability.Active && Setup.DetectsBlockImmunity)
            {
                var notification = Notification.Chat("Your target couldn't be role-blocked!");
                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), new TargetNotification
            {
                UserAddMessage = target => $"You will role-block {target.Name}.",
                UserRemoveMessage = target => "You won't role-block anyone.",
                UserChangeMessage = (old, current) => $"You will instead role-block {current.Name}."
            });
        }
    }

    public class ConsortSetup : MafiaMinionSetup, IRoleBlockImmune
    {
        public bool DetectsBlockImmunity = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}