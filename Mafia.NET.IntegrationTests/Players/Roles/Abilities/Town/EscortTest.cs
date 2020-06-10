using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Block))]
    public class EscortTest : BaseMatchTest
    {
        [TestCaseSource(typeof(BlockCases))]
        public void Block(string rolesString, bool block)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SerialKillerSetup
            {
                KillsRoleBlockers = false
            });
            match.Start();

            var blocker = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var attacker = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (block) blocker.Target(attacker);
            attacker.Target(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(citizen.Alive, Is.EqualTo(block));
            Deaths(match, block ? 0 : 1);
        }
    }

    public class BlockCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var blockers = new[] {"Escort", "Consort"};

            foreach (var blocker in blockers)
            {
                var roleNames = $"{blocker},Citizen,Serial Killer,Mafioso";

                foreach (var block in new[] {true, false})
                    yield return new object[] {roleNames, block};
            }
        }
    }
}