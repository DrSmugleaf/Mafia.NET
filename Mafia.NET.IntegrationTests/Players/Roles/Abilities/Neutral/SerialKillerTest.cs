using System.Collections.Generic;
using System.Linq;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(SerialKiller))]
    public class SerialKillerTest : BaseMatchTest
    {
        [TestCase("Serial Killer,Citizen", true, false)]
        [TestCase("Serial Killer,Citizen", false, true)]
        [TestCase("Serial Killer,Godfather", true, true)]
        [TestCase("Serial Killer,Godfather", false, true)]
        public void Kill(string rolesString, bool attack, bool survived)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var sk = match.AllPlayers[0];
            var other = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (attack) sk.Target(other);

            match.Skip<DeathsPhase>();

            Assert.That(sk.Alive, Is.True);
            Assert.That(other.Alive, Is.EqualTo(survived));
            Deaths(match, survived ? 0 : 1);
        }

        [TestCase("Serial Killer,Escort,Mafioso", true, true, false, true, true)]
        [TestCase("Serial Killer,Escort,Mafioso", true, true, true, false, false)]
        [TestCase("Serial Killer,Escort,Mafioso", true, false, true, false, true)]
        [TestCase("Serial Killer,Escort,Mafioso", true, false, true, false, false)]
        [TestCase("Serial Killer,Escort,Mafioso", false, true, false, true, true)]
        [TestCase("Serial Killer,Escort,Mafioso", false, true, true, true, false)]
        [TestCase("Serial Killer,Escort,Mafioso", false, false, true, true, true)]
        [TestCase("Serial Killer,Escort,Mafioso", false, false, true, true, false)]
        public void Blocked(
            string rolesString,
            bool attack,
            bool block,
            bool blockerSurvived,
            bool defenderSurvived,
            bool killsBlockers)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SerialKillerSetup {KillsRoleBlockers = killsBlockers});
            match.Start();

            var sk = match.AllPlayers[0];
            var blocker = match.AllPlayers[1];
            var other = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (attack) sk.Target(other);
            if (block) blocker.Target(sk);

            match.Skip<DeathsPhase>();

            Assert.That(sk.Alive, Is.True);
            Assert.That(blocker.Alive, Is.EqualTo(blockerSurvived));
            Assert.That(other.Alive, Is.EqualTo(defenderSurvived));

            Deaths(match, new[] {blockerSurvived, defenderSurvived}.Count(b => !b));
        }

        [TestCase("Serial Killer,Investigator,Citizen,Citizen", true, true, false)]
        [TestCase("Serial Killer,Investigator,Citizen,Citizen", true, false, true)]
        [TestCase("Serial Killer,Investigator,Citizen,Citizen", false, true, false)]
        [TestCase("Serial Killer,Investigator,Citizen,Citizen", false, false, false)]
        public void Investigator(string rolesString, bool attack, bool immune, bool crime)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SerialKillerSetup {DetectionImmune = immune});
            match.Start();

            var sk = match.AllPlayers[0];
            var investigator = match.AllPlayers[1];
            var other = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (attack) sk.Target(other);
            investigator.Target(sk);

            var notifications = new List<Text>();
            investigator.Chat += (s, e) => notifications.Add(e);

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.Positive);
            var detection = crime
                ? sk.Crimes.Crime(investigator.Ability, InvestigateKey.Detect)
                : Notification.Chat(CrimeKey.NotGuilty, sk);

            Assert.That(notifications[0].ToString(), Is.EqualTo(detection.ToString()));

            Assert.That(sk.Alive, Is.True);
            Assert.That(investigator.Alive, Is.True);
            Assert.That(other.Alive, Is.Not.EqualTo(attack));

            Deaths(match, attack ? 1 : 0);
        }

        [TestCase("Serial Killer,Serial Killer,Citizen,Citizen", AttackStrength.Base, true)]
        [TestCase("Serial Killer,Serial Killer,Citizen,Citizen", AttackStrength.None, false)]
        public void NightImmune(string rolesString, AttackStrength immunity, bool survived)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SerialKillerSetup {NightImmunity = (int) immunity});
            match.Start();

            var sk = match.AllPlayers[0];
            var attacker = match.AllPlayers[1];

            match.Skip<NightPhase>();

            attacker.Target(sk);

            match.Skip<DeathsPhase>();

            Assert.That(sk.Alive, Is.EqualTo(survived));
            Assert.That(attacker.Alive, Is.True);

            Deaths(match, survived ? 0 : 1);
        }
    }
}