using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum HideKey
    {
        SomeoneHide,
        SelfHide,
        HideAt
    }

    public class Hide : AbilityAction<IHideSetup>
    {
        public Hide(
            IAbility<IHideSetup> ability,
            int priority = 2,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (Uses == 0) return false;

            User.Crimes.Add(CrimeKey.Trespassing);

            foreach (var player in Match.LivingPlayers)
            {
                var targets = player.TargetManager;
                if (targets[0] == User) targets.ForceSet(target);
            }

            if (Setup.NotifiesTarget)
                target.OnNotification(Notification.Chat(Ability, HideKey.SomeoneHide));
            
            var notification = target == User
                ? Notification.Chat(Ability, HideKey.SelfHide)
                : Notification.Chat(Ability, HideKey.HideAt, target); // TODO: Attribute kills to the Beguiler

            User.OnNotification(notification);

            Ability.Uses--;

            return true;
        }
    }

    public interface IHideSetup : IUsesSetup
    {
        public bool NotifiesTarget { get; set; }
    }
}