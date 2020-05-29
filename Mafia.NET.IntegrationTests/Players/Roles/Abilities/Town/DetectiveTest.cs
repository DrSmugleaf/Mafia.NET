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
    [TestOf(typeof(Detective))]
    public class DetectiveTest : BaseMatchTest
    {
        [TestCase("Detective,Citizen,Mafioso", true, true)]
        [TestCase("Detective,Citizen,Mafioso", false, true)]
        [TestCase("Detective,Citizen,Godfather", true, true)]
        [TestCase("Detective,Citizen,Godfather", false, false)]
        public void Detect(string namesString, bool ignores, bool detected)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new DetectiveSetup {IgnoresDetectionImmunity = ignores});
            match.Start();

            var detective = match.AllPlayers[0];
            var innocent = match.AllPlayers[1];
            var mafia = match.AllPlayers[2];

            match.Skip<NightPhase>();

            detective.Target(mafia);
            mafia.Target(innocent);

            var notifications = new List<Text>();
            detective.Chat += (s, e) => notifications.Add(e);

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.Positive);

            var message = detected
                ? Notification.Chat(DetectiveKey.TargetVisitedSomeone, innocent)
                : Notification.Chat(DetectiveKey.TargetInactive);
            var localized = message.Localize();
            Assert.That(notifications[0], Is.EqualTo(localized));
        }
    }
}