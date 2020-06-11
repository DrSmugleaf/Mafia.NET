using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Watch))]
    public class LookoutTest : BaseMatchTest
    {
        [TestCaseSource(typeof(LookoutCases))]
        public void Detect(string rolesString, bool visit, bool immune, bool ignoresImmunity)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            if (!immune) match.Perks[roleNames[2]].DetectionImmune = false;
            match.AbilitySetups.Set(new WatchSetup
            {
                IgnoresDetectionImmunity = ignoresImmunity
            });
            match.Start();

            var lookout = match.AllPlayers[0];
            var visited = match.AllPlayers[1];
            var visitor = match.AllPlayers[2];

            match.Skip<NightPhase>();

            lookout.Target(visited);

            if (visit) visitor.Target(visited);

            var notifications = new List<Text>();
            lookout.Chat += (s, e) => notifications.Add(e);

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.Positive);
            var message = visit && (!immune || ignoresImmunity)
                ? Notification.Chat(lookout.Role, WatchKey.SomeoneVisitedTarget, visitor)
                : Notification.Chat(lookout.Role, WatchKey.NoneVisitedTarget);
            var localized = message.Localize();
            Assert.That(notifications[0], Is.EqualTo(localized));
        }
    }

    public class LookoutCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roles = new Dictionary<string, bool>
            {
                ["Mafioso"] = false,
                ["Enforcer"] = false,
                ["Godfather"] = true
            };

            foreach (var role in roles)
            {
                var roleNames = $"Lookout,Citizen,{role.Key}";
                var canBeImmune = role.Value;

                foreach (var visit in new[] {true, false})
                foreach (var immune in new[] {true, false})
                foreach (var ignoresImmunity in new[] {true, false})
                {
                    if (immune && !canBeImmune) continue;

                    yield return new object[] {roleNames, immune, visit, ignoresImmunity};
                }
            }
        }
    }
}