﻿using Mafia.NET.Localization;
using Mafia.NET.Notifications;

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

            User.Crimes.Add(CrimeKey.Trespassing);

            var targetVisitMessage = Notification.Chat(AgentKey.TargetInactive);
            if (target.Role.Ability.DetectTarget(out var targetVisit))
                targetVisitMessage = Notification.Chat(AgentKey.TargetVisitedSomeone, targetVisit);

            var foreignVisits = new EntryBundle();
            foreach (var other in Match.LivingPlayers)
            {
                var foreignVisit = other.Role.Ability.TargetManager[0];
                if (foreignVisit != target) continue;

                foreignVisits.Chat(AgentKey.SomeoneVisitedTarget, other);
            }

            if (foreignVisits.Entries.Count == 0) foreignVisits.Chat(AgentKey.NoneVisitedTarget);

            User.OnNotification(targetVisitMessage);
            User.OnNotification(foreignVisits);
        }

        protected override void _onNightStart()
        {
            if (Cooldown > 0) return;

            AddTarget(TargetFilter.Living(Match), TargetNotification.Enum<AgentKey>());
        }
    }

    public class AgentSetup : MafiaMinionSetup, ICooldownSetup
    {
        public int NightsBetweenUses { get; set; } = 1;
    }
}