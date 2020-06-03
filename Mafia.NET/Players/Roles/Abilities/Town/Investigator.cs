using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum InvestigatorKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Investigator", typeof(InvestigatorSetup))]
    public class Investigator : TownAbility<InvestigatorSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var investigative = new Investigate(this);
            actions.Add(investigative);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User),
                TargetNotification.Enum<InvestigatorKey>());
        }
    }

    public class InvestigatorSetup : ITownSetup, IInvestigativeSetup
    {
        public bool DetectsExactRole { get; set; } = false;
    }
}