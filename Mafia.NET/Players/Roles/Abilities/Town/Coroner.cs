using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CoronerKey
    {
        StillAlive,
        AutopsyRole,
        AutopsyLastWill,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Coroner", typeof(CoronerSetup))]
    public class Coroner : TownAbility<CoronerSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var autopsy = new Autopsy(this);
            actions.Add(autopsy);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Dead(Match), TargetNotification.Enum<CoronerKey>());
        }
    }

    public class CoronerSetup : ITownSetup, IAutopsySetup
    {
        public bool DiscoverAllTargets { get; set; } = true;
        public bool DiscoverDeathType { get; set; } = true;
        public bool DiscoverLastWill { get; set; } = true;
    }
}