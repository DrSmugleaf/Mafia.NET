using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Consigliere", typeof(ConsigliereSetup))]
    public class Consigliere : MafiaAbility<ConsigliereSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            User.Crimes.Add("Trespassing");

            var message = Setup.ExactDetection
                ? $"{target.Name} is a {target.Crimes.RoleName()}."
                : $"{target.Name} is guilty of {target.Crimes.Crime()}.";
            var notification = Notification.Chat(message);

            User.OnNotification(notification);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), new TargetNotification
            {
                UserAddMessage = target => $"You will investigate {target.Name}.",
                UserRemoveMessage = target => "You won't investigate anyone.",
                UserChangeMessage = (old, current) => $"You will instead investigate {current.Name}."
            });
        }
    }

    public class ConsigliereSetup : MafiaSuperMinionSetup
    {
        public bool ExactDetection = false;
    }
}