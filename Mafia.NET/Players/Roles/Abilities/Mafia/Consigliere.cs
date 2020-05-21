using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum ConsigliereKey
    {
        ExactDetect,
        Detect,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Consigliere", typeof(ConsigliereSetup))]
    public class Consigliere : MafiaAbility<ConsigliereSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            User.Crimes.Add("Trespassing");

            // TODO: Target switch
            var message = Setup.ExactDetection
                ? Notification.Chat(ConsigliereKey.ExactDetect, target, target.Crimes.RoleName())
                : Notification.Chat(ConsigliereKey.Detect, target, target.Crimes.Crime());

            User.OnNotification(message);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                TargetNotification.Enum<ConsigliereKey>());
        }
    }

    public class ConsigliereSetup : MafiaSuperMinionSetup
    {
        public bool ExactDetection = false;
    }
}