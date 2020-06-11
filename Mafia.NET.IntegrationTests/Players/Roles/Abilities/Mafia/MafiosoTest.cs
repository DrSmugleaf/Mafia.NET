using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(MafiaSuggest))]
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
            var mafiosos = new[] {"Mafioso", "Enforcer"};

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
            var combinations = new Dictionary<string, string>
            {
                ["Mafioso"] = "Godfather",
                ["Enforcer"] = "Dragon Head"
            };

            foreach (var combination in combinations)
            {
                var suggester = combination.Key;
                var head = combination.Value;
                var roleNames = $"{suggester},{head},Citizen,Citizen";

                foreach (var suggest in new[] {true, false})
                foreach (var gfAttack in new[] {true, false})
                    yield return new object[] {roleNames, suggest, gfAttack};
            }
        }
    }
}