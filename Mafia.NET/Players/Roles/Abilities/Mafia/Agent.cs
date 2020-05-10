using System;
using System.Collections.Generic;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Agent", typeof(AgentSetup))]
    public class Agent : MafiaAbility<AgentSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            if (Cooldown > 0)
            {
                Cooldown--;
                return;
            }

            Cooldown = Setup.NightsBetweenUses;

            User.Crimes.Add("Trespassing");

            var targetVisitMessage = "Your target did not do anything tonight.";
            if (target.Role.Ability.DetectTarget(out var targetVisit))
                targetVisitMessage = $"Your target visited {targetVisit.Name} tonight.";

            var foreignVisits = new List<string>();
            foreach (var player in Match.LivingPlayers)
            {
                var foreignVisit = player.Role.Ability.TargetManager[0];
                if (foreignVisit != target) continue;

                foreignVisits.Add($"{player.Name} visited your target tonight.");
            }

            if (foreignVisits.Count == 0) foreignVisits.Add("Your target was not visited by anyone tonight.");

            var targetNotification = Notification.Chat(targetVisitMessage);
            var foreignNotification = Notification.Chat(string.Join(Environment.NewLine, foreignVisits));

            User.OnNotification(targetNotification);
            User.OnNotification(foreignNotification);
        }

        protected override void _onNightStart()
        {
            if (Cooldown > 0) return;

            AddTarget(TargetFilter.Living(Match), new TargetNotification
            {
                UserAddMessage = target => $"You will watch {target.Name}.",
                UserRemoveMessage = target => "You won't watch anyone.",
                UserChangeMessage = (old, current) => $"You will instead watch {current.Name}."
            });
        }
    }

    public class AgentSetup : MafiaMinionSetup, ICooldownSetup
    {
        public int NightsBetweenUses { get; set; } = 1;
    }
}