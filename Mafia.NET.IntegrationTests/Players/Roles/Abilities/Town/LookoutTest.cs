using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Lookout))]
    public class LookoutTest : BaseMatchTest
    {
        [TestCase("Lookout,Citizen,Mafioso", true)]
        [TestCase("Lookout,Citizen,Mafioso", false)]
        public void Detect(string namesString, bool visit)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
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
            var message = visit
                ? Notification.Chat(LookoutKey.SomeoneVisitedTarget, visitor)
                : Notification.Chat(LookoutKey.NoneVisitedTarget);
            var localized = message.Localize();
            Assert.That(notifications[0], Is.EqualTo(localized));
        }
    }
}