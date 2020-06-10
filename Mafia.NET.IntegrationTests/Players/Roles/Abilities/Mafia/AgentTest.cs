using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Agent))]
    public class AgentTest : BaseMatchTest
    {
        [TestCaseSource(typeof(AgentCases))]
        public void Inspect(string rolesString, bool firstInnocent, bool immune, bool ignoresImmunity)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new AgentSetup
            {
                IgnoresDetectionImmunity = ignoresImmunity
            });
            match.Roles["Serial Killer"].DetectionImmune = immune;
            match.Start();

            var agent = match.AllPlayers[0];
            var first = match.AllPlayers[1];
            var second = match.AllPlayers[2];

            match.Skip<NightPhase>();

            agent.Target(first);
            Assert.That(agent.Targets[0], Is.Not.Null);

            first.Target(second);
            second.Target(first);

            var notifications = new List<string>();
            agent.Chat += (s, e) => notifications.Add(e.ToString());

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.EqualTo(2));

            var targetVisited = Notification.Chat(agent.Role, DetectKey.TargetVisitedSomeone, second).ToString();
            var targetInactive = Notification.Chat(agent.Role, DetectKey.TargetInactive).ToString();
            var otherVisited = Notification.Chat(agent.Role, WatchKey.SomeoneVisitedTarget, second).ToString();
            var noneVisited = Notification.Chat(agent.Role, WatchKey.NoneVisitedTarget).ToString();

            if (firstInnocent || !immune || ignoresImmunity)
                Assert.That(notifications, Does.Contain(targetVisited));
            else Assert.That(notifications, Does.Contain(targetInactive));

            if (!immune || ignoresImmunity)
                Assert.That(notifications, Does.Contain(otherVisited));
            else Assert.That(notifications, Does.Contain(noneVisited));

            match.Skip<NightPhase>();

            agent.Target(first);
            Assert.That(agent.Targets[0], Is.Null);

            match.Skip<DeathsPhase>();
        }
    }

    public class AgentCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roles = new Dictionary<string, bool>
            {
                ["Doctor"] = true,
                ["Serial Killer"] = false
            };

            foreach (var role in roles)
            {
                var roleNames = $"Agent,{role.Key},Serial Killer,Mafioso";
                var innocent = role.Value;

                foreach (var immune in new[] {true, false})
                foreach (var ignoresImmunity in new[] {true, false})
                    yield return new object[] {roleNames, innocent, immune, ignoresImmunity};
            }
        }
    }
}