using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.HealProfiles;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Guard))]
    public class BodyguardTest : BaseMatchTest
    {
        [TestCase("Bodyguard,Citizen,Mafioso", true, false)]
        [TestCase("Bodyguard,Citizen,Mafioso", false, false)]
        [TestCase("Bodyguard,Citizen,Godfather", true, false)]
        [TestCase("Bodyguard,Citizen,Godfather", false, true)]
        public void Piercing(string rolesString, bool piercing, bool attackerAlive)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Replace(new GuardSetup
            {
                IgnoresInvulnerability = piercing
            });
            match.Start();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var attacker = match.AllPlayers[2];

            match.Skip<NightPhase>();

            bodyguard.Target(citizen);
            attacker.Target(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.False);
            Assert.That(citizen.Alive, Is.True);
            Assert.That(attacker.Alive, Is.EqualTo(attackerAlive));
        }

        [TestCase("Bodyguard,Citizen,Doctor,Godfather", true)]
        [TestCase("Bodyguard,Citizen,Doctor,Godfather", false)]
        public void Healing(string rolesString, bool healing)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            if (healing) match.Perks["Bodyguard"].SetHeal<HealProfile>();
            match.Start();

            match.Skip<NightPhase>();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var doctor = match.AllPlayers[2];
            var godfather = match.AllPlayers[3];

            bodyguard.Target(citizen);
            doctor.Target(bodyguard);
            godfather.Target(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.EqualTo(healing));
            Assert.That(citizen.Alive, Is.True);
            Assert.That(doctor.Alive, Is.True);
            Assert.That(godfather.Alive, Is.False);
        }
    }
}