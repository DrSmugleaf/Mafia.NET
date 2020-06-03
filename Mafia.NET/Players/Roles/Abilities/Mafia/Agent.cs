using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum AgentKey
    {
        TargetInactive,
        TargetVisitedSomeone,
        SomeoneVisitedTarget,
        NoneVisitedTarget,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Agent", typeof(AgentSetup))]
    public class Agent : MafiaAbility<AgentSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var agent = new AgentAction(this);
            actions.Add(agent);
        }

        protected override void _onNightStart()
        {
            if (Cooldown > 0) return;

            AddTarget(TargetFilter.Living(Match), TargetNotification.Enum<AgentKey>());
        }
    }

    public class AgentSetup : MafiaMinionSetup, IAgentSetup
    {
        public int NightsBetweenUses { get; set; } = 1;
        public bool IgnoresDetectionImmunity { get; set; } = false;
    }
}