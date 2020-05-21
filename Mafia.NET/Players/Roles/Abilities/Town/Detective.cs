using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum DetectiveKey
    {
        TargetInactive,
        TargetVisitedSomeone,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Detective", typeof(DetectiveSetup))]
    public class Detective : TownAbility<DetectiveSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            Notification notification;
            if (target.Role.Ability.DetectTarget(out var visited, Setup))
                notification = Notification.Chat(DetectiveKey.TargetVisitedSomeone, visited);
            else notification = Notification.Chat(DetectiveKey.TargetInactive);

            User.OnNotification(notification);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<DetectiveKey>());
        }
    }

    public class DetectiveSetup : ITownSetup, IIgnoresDetectionImmunity
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}