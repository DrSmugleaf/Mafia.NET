using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Consort", typeof(ConsortSetup))]
    public class Consort : MafiaAbility<ConsortSetup>
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will role-block {target.Name}.",
                UserRemoveMessage = (target) => $"You won't role-block anyone.",
                UserChangeMessage = (old, _new) => $"You will instead role-block {_new.Name}."
            });
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.Try(0, out var target))
            {
                User.Crimes.Add("Soliciting");
                if (target.Role.Affiliation.Name == "Town") User.Crimes.Add("Disturbing the peace");

                target.Role.Ability.Disable();
                if (target.Role.Ability.Active && Setup.DetectsBlockImmunity)
                {
                    var notification = Notification.Chat("Your target couldn't be role-blocked!");
                    User.OnNotification(notification);
                }

                return true;
            }

            return false;
        }
    }

    public class ConsortSetup : MafiaMinionSetup, IRoleBlockImmune
    {
        public bool RoleBlockImmune { get; set; } = false;
        public bool DetectsBlockImmunity = false;
    }
}
