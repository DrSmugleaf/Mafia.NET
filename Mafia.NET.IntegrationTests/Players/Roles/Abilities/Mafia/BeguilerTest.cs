using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Beguiler))]
    public class BeguilerTest : BaseMatchTest
    {
        [TestCaseSource(typeof(HideKillCases))]
        public void HideKill(string rolesString, bool hide, bool kill, bool canHideBehindMafia, bool notifiesTarget)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new BeguilerSetup
            {
                CanHideBehindMafia = canHideBehindMafia,
                NotifiesTarget = notifiesTarget
            });
            match.Start();

            var beguiler = match.AllPlayers[0];
            var visitor = match.AllPlayers[1];
            var visited = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (hide) beguiler.Target(visited);
            if (kill) visitor.Target(beguiler);

            var notifications = new List<string>();
            visited.Chat += (s, e) => notifications.Add(e.ToString());

            match.Skip<DeathsPhase>();

            Deaths(match, kill ? 1 : 0);
            Assert.That(beguiler.Alive, Is.EqualTo(hide || !kill));
            Assert.That(visitor.Alive, Is.True);
            Assert.That(visited.Alive, Is.EqualTo(!hide || !kill));

            var hideNotification = Notification.Chat(beguiler.Ability, HideKey.SomeoneHide).ToString();
            Assert.That(notifications, hide && notifiesTarget
                ? Does.Contain(hideNotification)
                : Does.Not.Contain(hideNotification));
        }
    }

    public class HideKillCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roleNames = "Beguiler,Serial Killer,Citizen,Mafioso";

            foreach (var hide in new[] {true, false})
            foreach (var kill in new[] {true, false})
            foreach (var canHideBehindMafia in new[] {true, false})
            foreach (var notifiesTarget in new[] {true, false})
                yield return new object[] {roleNames, hide, kill, canHideBehindMafia, notifiesTarget};
        }
    }
}