using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum VigilanteKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetImmune,
        FirstNight
    }

    [RegisterAbility("Vigilante", typeof(VigilanteSetup))]
    public class Vigilante : TownAbility<VigilanteSetup>
    {
        public TargetNotification TargetMessage()
        {
            return Match.Phase.Day == 1 ?
                new TargetNotification() 
                {
                    UserAddMessage = target =>
                    {
                        TargetManager.ForceSet(null);
                        return Notification.Chat(VigilanteKey.FirstNight);
                    }
                }
                : TargetNotification.Enum<VeteranKey>();
        }

        public override void Kill()
        {
            if (!TargetManager.Try(out var target) || Uses == 0) return;

            Uses--;

            if (!Attack(target))
            {
                var notification = Notification.Chat(VigilanteKey.TargetImmune);
                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match).Except(User), TargetMessage());
        }
    }

    public class VigilanteSetup : ITownSetup, IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}