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
    public class Consigliere : MafiaAbility<ConsigliereSetup>
    {
        public override void Detect()
        {
            if (!TargetManager.Try(out var target)) return;

            User.Crimes.Add(CrimeKey.Trespassing);

            // TODO: Target switch
            var message = Setup.DetectsExactRole
                ? Notification.Chat(ConsigliereKey.ExactDetect, target, target.Crimes.RoleName())
                : target.Crimes.Crime(ConsigliereKey.Detect);

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
        public bool DetectsExactRole = false;
    }
}