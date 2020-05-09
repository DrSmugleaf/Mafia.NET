using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Consigliere", typeof(ConsigliereSetup))]
    public class Consigliere : MafiaAbility<ConsigliereSetup>
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will investigate {target.Name}.",
                UserRemoveMessage = (target) => "You won't investigate anyone.",
                UserChangeMessage = (old, _new) => $"You will instead investigate {_new.Name}."
            });
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.Try(0, out var target))
            {
                User.Crimes.Add("Trespassing");

                string message = Setup.ExactDetection ?
                    $"{target.Name} is a {target.Crimes.RoleName()}." :
                    $"{target.Name} is guilty of {target.Crimes.Crime()}.";
                var notification = Notification.Chat(message);
                User.OnNotification(notification);

                return true;
            }

            return false;
        }
    }

    public class ConsigliereSetup : MafiaSuperMinionSetup
    {
        public bool ExactDetection = false;
    }
}
