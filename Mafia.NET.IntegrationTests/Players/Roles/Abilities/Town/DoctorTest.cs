using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Heal))]
    public class DoctorTest : BaseMatchTest
    {
        [TestCase("Doctor,Citizen,Mafioso", true, true)]
        [TestCase("Doctor,Citizen,Mafioso", false, false)]
        [TestCase("Doctor,Citizen,Mafioso,Mafioso", true, true)]
        [TestCase("Doctor,Citizen,Mafioso,Mafioso", false, false)]
        public void Heal(string rolesString, bool heal, bool alive)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var doctor = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (heal) doctor.Target(citizen);

            for (var i = 2; i < match.AllPlayers.Count; i++)
            {
                var attacker = match.AllPlayers[i];
                attacker.Target(citizen);
            }

            match.Skip<DeathsPhase>();

            Assert.That(doctor.Alive, Is.True);
            Assert.That(citizen.Alive, Is.EqualTo(alive));

            Deaths(match, alive ? 0 : 1);
        }
    }
}