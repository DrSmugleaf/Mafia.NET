using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum LookoutKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        SomeoneVisitedTarget,
        NoneVisitedTarget
    }

    [RegisterAbility("Lookout", typeof(LookoutSetup))]
    public class Lookout : TownAbility<LookoutSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var watch = new Watch(this);
            actions.Add(watch);
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf)
                filter = filter.Except(User);

            AddTarget(filter, TargetNotification.Enum<LookoutKey>());
        }
    }

    public class LookoutSetup : ITownSetup, IDetectSetup
    {
        public bool CanTargetSelf = false;
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}