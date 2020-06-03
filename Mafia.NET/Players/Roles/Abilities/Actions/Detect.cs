using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum DetectKey
    {
        TargetInactive,
        TargetVisitedSomeone
    }

    public class Detect : AbilityAction<IDetectSetup>
    {
        public Detect(
            IAbility<IDetectSetup> ability,
            int priority = 9,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            var notification = target.Role.Ability.DetectTarget(out var visited, Setup)
                ? Notification.Chat(Ability, DetectKey.TargetVisitedSomeone, visited)
                : Notification.Chat(Ability, DetectKey.TargetInactive);

            User.OnNotification(notification);

            return true;
        }
    }

    public interface IDetectSetup : IAbilitySetup
    {
        public bool IgnoresDetectionImmunity { get; set; }
    }
}