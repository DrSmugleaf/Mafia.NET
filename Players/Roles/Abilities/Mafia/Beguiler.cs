using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Beguiler", typeof(BeguilerSetup))]
    public class Beguiler : MafiaAbility<BeguilerSetup>
    {
        protected override void _onNightStart()
        {
            if (Charges == 0) return;

            TargetFilter filter = Setup.CanHideBehindMafia ?
                TargetFilter.Living(Match).Except(User) :
                TargetFilter.Living(Match).Except(User.Role.Affiliation);

            AddTarget(filter, new TargetMessage()
            {
                UserAddMessage = (target) => $"You will hide behind {target.Name}.",
                UserRemoveMessage = (target) => $"You won't hide behind anyone.",
                UserChangeMessage = (old, _new) => $"You will instead hide behind ${_new.Name}."
            });
        }

        protected override bool _onNightEnd()
        {
            if (TargetManager.Try(0, out var target) && Charges > 0)
            {
                Charges--;

                foreach (var player in Match.LivingPlayers.Values)
                {
                    var targets = player.Role.Ability.TargetManager;
                    if (targets[0] == User)
                    {
                        targets.ForceSet(target);
                        if (Setup.NotifiesTarget) target.OnNotification(Notification.Chat("Someone hid in your house tonight."));
                    }
                }

                Notification notification;
                if (target == User)
                {
                    notification = Notification.Chat("You cower in the corner, hiding behind yourself.");
                }
                else
                {
                    notification = Notification.Chat($"You went to hide behind {target.Name}");
                }

                User.OnNotification(notification);

                return true;
            }

            return false;
        }
    }

    public class BeguilerSetup : MafiaMinionSetup, IChargeSetup
    {
        public int Charges { get; set; } = 2;
        public bool NotifiesTarget = false;
        public bool CanHideBehindMafia = false;
    }
}
