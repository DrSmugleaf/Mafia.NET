using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

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
    public class Detective : TownAbility<DetectiveSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var detect = new Detect(this);
            actions.Add(detect);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<DetectiveKey>());
        }
    }

    public class DetectiveSetup : ITownSetup, IDetectSetup
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}