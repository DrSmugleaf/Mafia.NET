using Mafia.NET.Matches.Chats;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Agent", typeof(AgentSetup))]
    public class Agent : MafiaAbility<AgentSetup>
    {
        protected override void _onNightStart()
        {
            TargetManager.Add(TargetFilter.Living(Match));
        }

        protected override void _onNightEnd()
        {
            if (TargetManager.Try(0, out var target))
            {
                var targetVisited = target.Role.Ability.TargetManager[0];
                var targetVisitedMessage = targetVisited == null ?
                    "Your target did not do anything tonight." :
                    $"Your target visited {targetVisited.Name} tonight.";

                var foreignVisits = new List<string>();
                foreach (var player in Match.LivingPlayers.Values)
                {
                    var visited = player.Role.Ability.TargetManager[0];
                    if (visited != target) continue;

                    foreignVisits.Add($"{player.Name} visited your target tonight.");
                }

                if (foreignVisits.Count == 0) foreignVisits.Add("Your target was not visited by anyone tonight.");

                var targetNotification = Notification.Chat(targetVisitedMessage);
                var foreignNotification = Notification.Chat(string.Join(Environment.NewLine, foreignVisits));

                User.OnNotification(targetNotification);
                User.OnNotification(foreignNotification);
            }
        }
    }

    public class AgentSetup : IAbilitySetup
    {
        public bool becomesMafiosoIfAlone = true;
        public int nightsBetweenShadowings = 1;
    }
}
