using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Investigator))]
    [TestOf(typeof(Consigliere))]
    public class InvestigatorTest : BaseMatchTest
    {
        [TestCaseSource(typeof(InvestigateCases))]
        public void Investigate(string rolesString, bool innocent, bool exact)
        {
            var roleNames = rolesString.Split(",");
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
                    innocent ? Is.EqualTo(citizenName) : Is.Not.EqualTo(citizenName));
            }
            else
            {
                Assert.That(target.Crimes.Crime(investigator.Ability, InvestigateKey.Detect).ToString(), innocent
                    ? Does.Contain("innocent")
                    : Does.Contain("guilty"));
            }
        }
    }

    public class InvestigateCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var investigators = new[] {"Investigator", "Consigliere"};
            var targets = new Dictionary<string, bool>
            {
                ["Citizen"] = true,
                ["Serial Killer"] = false
            };

            foreach (var investigator in investigators)
            foreach (var target in targets)
            {
                var targetRole = target.Key;
                var roleNames = $"{investigator},Citizen,{targetRole},Mafioso";
                var innocent = target.Value;

                foreach (var exact in new[] {true, false})
                    yield return new object[] {roleNames, innocent, exact};
            }
        }
    }
}