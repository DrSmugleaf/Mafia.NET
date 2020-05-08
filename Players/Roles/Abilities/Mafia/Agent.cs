using Mafia.NET.Matches.Chats;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Agent", typeof(AgentSetup))]
    public class Agent : MafiaAbility<AgentSetup>
    {
        public Agent()
        {
            Cooldown = Setup.nightsBetweenShadowings;
        }

        protected override void _onDayStart()
        {
            if (AloneTeam() && Setup.becomesMafiosoIfAlone)
            {
                User.Role.Ability = AbilityRegistry.Instance.Ability<Mafioso>(Match, User);
            }
        }

        protected override void _onNightStart()
        {
            if (Cooldown == 0)
            {
                AddTarget(TargetFilter.Living(Match), new TargetMessage()
                {
                    UserAddMessage = (target) => $"You will watch {target.Name}.",
                    UserRemoveMessage = (target) => $"You won't watch {target.Name}.",
                    UserChangeMessage = (old, _new) => $"You will instead watch ${_new.Name}."
                });
            }
        }

        protected override void _onNightEnd()
        {
            if (Cooldown == 0 && TargetManager.Try(0, out var target))
            {
                Cooldown = Setup.nightsBetweenShadowings;
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
            else
            {
                Cooldown--;
            }
        }
    }

    public class AgentSetup : IAbilitySetup
    {
        public bool becomesMafiosoIfAlone = true;
        public int nightsBetweenShadowings = 1;
    }
}
