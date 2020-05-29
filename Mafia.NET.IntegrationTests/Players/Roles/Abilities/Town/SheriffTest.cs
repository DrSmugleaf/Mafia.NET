using System;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Sheriff))]
    public class SheriffTest : BaseMatchTest
    {
        public void Detect(IMatch match, Key result)
        {
            var sheriff = match.AllPlayers[0];
            var other = match.AllPlayers[1];

            match.Skip<NightPhase>();

            sheriff.Target(other);

            var notifications = new List<Text>();
            sheriff.Chat += (s, e) => notifications.Add(e);

            match.Skip<DeathsPhase>();

            Assert.That(notifications.Count, Is.Positive);

            var localized = result.Localize();
            Assert.That(notifications[0], Is.EqualTo(localized));
        }

        [TestCase("Sheriff,Citizen", true, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Citizen", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Mafioso", true, SheriffKey.Mafia)]
        [TestCase("Sheriff,Mafioso", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Arsonist", true, SheriffKey.Arsonist)]
        [TestCase("Sheriff,Arsonist", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Mass Murderer", true, SheriffKey.MassMurderer)]
        [TestCase("Sheriff,Mass Murderer", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Serial Killer", true, SheriffKey.SerialKiller)]
        [TestCase("Sheriff,Serial Killer", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Cultist", true, SheriffKey.Cultist)]
        [TestCase("Sheriff,Cultist", false, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Witch Doctor", true, SheriffKey.Cultist)]
        [TestCase("Sheriff,Witch Doctor", false, SheriffKey.NotSuspicious)]
        public void DetectVulnerable(string namesString, bool detects, Enum result)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SheriffSetup
            {
                DetectsArsonist = detects,
                DetectsCult = detects,
                DetectsMafiaTriad = detects,
                DetectsSerialKiller = detects,
                DetectsMassMurderer = detects
            });

            match.Start();
            Detect(match, result);
        }


        [TestCase("Sheriff,Godfather", true, true, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Godfather", true, false, SheriffKey.Mafia)]
        [TestCase("Sheriff,Godfather", false, true, SheriffKey.NotSuspicious)]
        [TestCase("Sheriff,Godfather", false, false, SheriffKey.NotSuspicious)]
        public void DetectImmune(string namesString, bool detects, bool immune, Enum result)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new SheriffSetup
            {
                DetectsArsonist = detects,
                DetectsCult = detects,
                DetectsMafiaTriad = detects,
                DetectsSerialKiller = detects,
                DetectsMassMurderer = detects
            });

            match.Start();

            var otherAbility = match.AllPlayers[1].Role.Ability;
            var setup = (IDetectionImmune) otherAbility.AbilitySetup;
            setup.DetectionImmune = immune;
            otherAbility.DetectionImmune = immune;

            Detect(match, result);
        }
    }
}