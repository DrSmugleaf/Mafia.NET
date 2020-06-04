using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Mafioso))]
    public class MafiosoTest : BaseMatchTest
    {
        [TestCaseSource(typeof(MafiosoAloneCases))]
        public void MafiosoAlone(string rolesString, bool attack)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var mafioso = match.AllPlayers[0];
            var target = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (attack) mafioso.Target(target);

            match.Skip<DeathsPhase>();

            Assert.That(mafioso.Alive, Is.True);
            Assert.That(target.Alive, Is.EqualTo(!attack));
            Deaths(match, attack ? 1 : 0);
        }

        [TestCaseSource(typeof(WithGodfatherCases))]
        public void WithGodfather(string rolesString, bool suggest, bool gfAttack)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var mafioso = match.AllPlayers[0];
            var godfather = match.AllPlayers[1];
            var first = match.AllPlayers[2];
            var second = match.AllPlayers[3];

            match.Skip<NightPhase>();

            if (suggest) mafioso.Target(first);
            if (gfAttack) godfather.Target(second);

            match.Skip<DeathsPhase>();

            Assert.That(mafioso.Alive, Is.True);
            Assert.That(godfather.Alive, Is.True);

            Assert.That(first.Alive, Is.EqualTo(!suggest || gfAttack));
            Assert.That(second.Alive, Is.EqualTo(!gfAttack));

            var attack = suggest || gfAttack;
            Deaths(match, attack ? 1 : 0);
        }
    }

    public class MafiosoAloneCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var mafiosos = new[] {"Mafioso"};

            foreach (var mafioso in mafiosos)
            {
                var roleNames = $"{mafioso},Citizen,Citizen";

                foreach (var attack in new[] {true, false})
                    yield return new object[] {roleNames, attack};
            }
        }
    }

    public class WithGodfatherCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var mafiosos = new[] {"Mafioso"};

            foreach (var mafioso in mafiosos)
            {
                var roleNames = $"{mafioso},Godfather,Citizen,Citizen";

                foreach (var suggest in new[] {true, false})
                foreach (var gfAttack in new[] {true, false})
                    yield return new object[] {roleNames, suggest, gfAttack};
            }
        }
    }
}