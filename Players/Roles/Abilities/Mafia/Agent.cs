using Mafia.NET.Matches.Chats;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Agent", typeof(AgentSetup))]
    public class Agent : MafiaAbility<AgentSetup>, IDetector
    {
        protected override void _onNightStart()
        {
            if (Cooldown > 0) return;

            AddTarget(TargetFilter.Living(Match), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will watch {target.Name}.",
                UserRemoveMessage = (target) => "You won't watch anyone.",
                UserChangeMessage = (old, _new) => $"You will instead watch {_new.Name}."
            });
        }

        public void Detect(IPlayer target)
        {
            if (Cooldown > 0)
            {
                Cooldown--;
                return;
            }

            Cooldown = Setup.NightsBetweenUses;

            User.Crimes.Add("Trespassing");

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

    public class AgentSetup : MafiaMinionSetup, ICooldownSetup
    {
        public int NightsBetweenUses { get; set; } = 1;
    }
}
