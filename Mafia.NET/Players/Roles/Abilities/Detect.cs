using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum DetectKey
    {
        TargetInactive,
        TargetVisitedSomeone,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Detect", 9, typeof(DetectSetup))]
    public class Detect : NightEndAbility<IDetectSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<DetectKey>(abilities, TargetFilter.Living(Match).Except(User));
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            var notification =
                target.Role.DetectionProfile.TryDetectTarget(out var visited, Setup)
                    ? Notification.Chat(Role, DetectKey.TargetVisitedSomeone, visited)
                    : Notification.Chat(Role, DetectKey.TargetInactive);

            User.OnNotification(notification);

            return true;
        }
    }

    public interface IDetectSetup : IAbilitySetup
    {
        bool IgnoresDetectionImmunity { get; set; }
    }

    public class DetectSetup : IDetectSetup
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}