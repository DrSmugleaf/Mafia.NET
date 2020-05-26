﻿using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Bodyguard))]
    public class BodyguardTest : BaseMatchTest
    {
        [TestCase("Bodyguard,Citizen,Mafioso")]
        [TestCase("Bodyguard,Citizen,Godfather")]
        public void AgainstVulnerable(string namesString)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            Deaths(match, 0);

            match.Skip<NightPhase>();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var attacker = match.AllPlayers[2];

            bodyguard.Role.Ability.TargetManager.Set(citizen);
            attacker.Role.Ability.TargetManager.Set(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.False);
            Assert.That(citizen.Alive, Is.True);
            Assert.That(attacker.Alive, Is.False);

            Deaths(match, 2);
        }

        [TestCase("Bodyguard,Citizen,Godfather")]
        public void AgainstImmune(string namesString)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new BodyguardSetup {IgnoresInvulnerability = false});
            match.Start();

            Deaths(match, 0);

            match.Skip<NightPhase>();

            var bodyguard = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var godfather = match.AllPlayers[2];

            bodyguard.Role.Ability.TargetManager.Set(citizen);
            godfather.Role.Ability.TargetManager.Set(citizen);

            match.Skip<DeathsPhase>();

            Assert.That(bodyguard.Alive, Is.False);
            Assert.That(citizen.Alive, Is.True);
            Assert.That(godfather.Alive, Is.True);

            Deaths(match, 1);
        }
    }
}