using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(Vest))]
    public class SurvivorTest : BaseMatchTest
    {
        public static readonly int DefaultUses = 3;

        [TestCaseSource(typeof(VestCases))]
        public void Vest(string rolesString, bool vest, bool attack, bool alive)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var survivor = match.AllPlayers[0];
            var killer = match.AllPlayers[1];
            var ability = survivor.Abilities.Get<Vest>();
            Assert.That(ability.Uses, Is.EqualTo(DefaultUses));

            match.Skip<NightPhase>();

            if (vest) survivor.Target(survivor);
            if (attack) killer.Target(survivor);

            Assert.That(ability.Uses, Is.EqualTo(DefaultUses));

            match.Skip<DeathsPhase>();

            Assert.That(survivor.Alive, Is.EqualTo(alive));
            Assert.That(killer.Alive, Is.True);
            Assert.That(ability.Uses, Is.EqualTo(vest ? DefaultUses - 1 : DefaultUses));
            Deaths(match, alive ? 0 : 1);
        }
    }

    public class VestCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roleNames = "Survivor,Serial Killer,Citizen";

            foreach (var vest in new[] {true, false})
            foreach (var attack in new[] {true, false})
                yield return new object[] {roleNames, vest, attack, vest || !attack};
        }
    }
}