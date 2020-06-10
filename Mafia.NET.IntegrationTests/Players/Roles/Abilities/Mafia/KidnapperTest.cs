using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Kidnap))]
    public class KidnapperTest : BaseMatchTest
    {
        [TestCaseSource(typeof(KidnapCases))]
        public void Kidnap(string rolesString, bool kidnap, bool lynch, bool execute, bool kidnapMafia)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new KidnapSetup
            {
                CanKidnapMafiaMembers = kidnapMafia
            });
            match.Start();

            var kidnapper = match.AllPlayers[0];
            var mafioso = match.AllPlayers[1];
            var lynched = match.AllPlayers[2];
            var citizen = match.AllPlayers[3];

            var accuse = match.Skip<AccusePhase>();

            if (kidnap) kidnapper.Target(mafioso);

            if (lynch)
                foreach (var player in match.AllPlayers)
                    accuse.AccuseManager.Accuse(player, lynched);

            match.Skip<NightPhase>();

            Assert.That(lynched.Alive, Is.EqualTo(!lynch));
            Deaths(match, lynch ? 1 : 0);

            if (execute) kidnapper.Target(mafioso);
            mafioso.Target(citizen);

            Assert.That(kidnapper.Targets[0], Is.Null);

            var jailed = !lynch && kidnap && kidnapMafia;
            Assert.That(mafioso.Targets[0], jailed ? Is.Null : Is.Not.Null);

            match.Skip<DeathsPhase>();

            Assert.That(lynched.Alive, Is.EqualTo(!lynch));
            Assert.That(kidnapper.Alive, Is.True);
            Assert.That(mafioso.Alive, Is.True);
            Assert.That(citizen.Alive, jailed ? (IResolveConstraint) Is.True : Is.False);
            Deaths(match, lynch ? 2 : jailed ? 0 : 1);
        }
    }

    public class KidnapCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var kidnappers = new[] {"Kidnapper"};

            foreach (var kidnapper in kidnappers)
            {
                var roleNames = $"{kidnapper},Mafioso,Citizen,Citizen,Citizen,Citizen";

                foreach (var kidnap in new[] {true, false})
                foreach (var lynch in new[] {true, false})
                foreach (var execute in new[] {true, false})
                foreach (var kidnapMafia in new[] {true, false})
                    yield return new object[] {roleNames, kidnap, lynch, execute, kidnapMafia};
            }
        }
    }
}