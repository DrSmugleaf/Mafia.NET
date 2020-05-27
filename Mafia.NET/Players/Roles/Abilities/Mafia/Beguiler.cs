using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum BeguilerKey
    {
        SomeoneHide,
        SelfHide,
        HideAt,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Beguiler", typeof(BeguilerSetup))]
    public class Beguiler : MafiaAbility<BeguilerSetup>
    {
        public override void Switch()
        {
            if (TargetManager.Try(out var target) && Uses > 0)
            {
                Uses--;
                User.Crimes.Add(CrimeKey.Trespassing);

                foreach (var player in Match.LivingPlayers)
                {
                    var targets = player.Role.Ability.TargetManager;
                    if (targets[0] == User)
                    {
                        targets.ForceSet(target);
                        if (Setup.NotifiesTarget)
                            target.OnNotification(Notification.Chat(BeguilerKey.SomeoneHide));
                    }
                }

                var notification = target == User
                    ? Notification.Chat(BeguilerKey.SelfHide)
                    : Notification.Chat(BeguilerKey.HideAt, target); // TODO: Attribute kills to the Beguiler

                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            var filter = Setup.CanHideBehindMafia
                ? TargetFilter.Living(Match).Except(User)
                : TargetFilter.Living(Match).Except(User.Role.Team);

            AddTarget(filter, TargetNotification.Enum<BeguilerKey>());
        }
    }

    public class BeguilerSetup : MafiaMinionSetup, IUsesSetup
    {
        public bool CanHideBehindMafia = false;
        public bool NotifiesTarget = false;
        public int Uses { get; set; } = 2;
    }
}