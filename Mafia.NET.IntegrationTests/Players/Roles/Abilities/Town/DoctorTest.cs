using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Doctor))]
    public class DoctorTest : BaseMatchTest
    {
        [TestCase("Doctor,Citizen,Mafioso", true)]
        [TestCase("Doctor,Citizen,Mafioso,Mafioso", true)]
        public void Heal(string namesString, bool alive)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            match.Skip<NightPhase>();

            var doctor = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            
            doctor.Role.Ability.TargetManager.Set(citizen);
            for (var i = 2; i < match.AllPlayers.Count; i++)
            {
                var attacker = match.AllPlayers[i];
                attacker.Role.Ability.TargetManager.Set(citizen);
            }

            match.Skip<DeathsPhase>();

            Assert.That(doctor.Alive, Is.True);
            Assert.That(citizen.Alive, Is.EqualTo(alive));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.LessThanOrEqualTo(1));
        }
    }
}