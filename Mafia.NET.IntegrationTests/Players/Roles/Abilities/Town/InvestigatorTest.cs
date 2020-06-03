using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Investigator))]
    public class InvestigatorTest : BaseMatchTest
    {
        [TestCase("Investigator,Citizen,Citizen", true, true)]
        [TestCase("Investigator,Citizen,Citizen", true, false)]
        [TestCase("Investigator,Citizen,Mafioso", false, true)]
        [TestCase("Investigator,Citizen,Mafioso", false, false)]
        public void Detect(string namesString, bool isInnocent, bool exact)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new InvestigatorSetup {DetectsExactRole = exact});
            match.Start();

            var investigator = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var target = match.AllPlayers[2];

            match.Skip<NightPhase>();

            investigator.Target(target);
            target.Target(citizen);

            var notifications = new List<Text>();
            investigator.Chat += (s, e) => notifications.Add(e);

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.Positive);

            if (exact)
            {
                var citizenName = citizen.Role.Name;
                Assert.That(target.Crimes.RoleName(),
                    isInnocent ? Is.EqualTo(citizenName) : Is.Not.EqualTo(citizenName));
            }
            else
            {
                Assert.That(target.Crimes.Crime(investigator.Ability, InvestigateKey.Detect).ToString(),
                    isInnocent ? Does.Contain("innocent") : Does.Contain("guilty"));
            }
        }
    }
}