using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town;

[TestFixture]
[TestOf(typeof(Sheriff))]
public class SheriffTest : BaseMatchTest
{
    public static SheriffKey Key(string team, string role)
    {
        return team switch
        {
            "Mafia" => SheriffKey.Mafia,
            "Triad" => SheriffKey.Triad,
            _ => role switch
            {
                "Arsonist" => SheriffKey.Arsonist,
                "Mass Murderer" => SheriffKey.MassMurderer,
                "Serial Killer" => SheriffKey.SerialKiller,
                "Cultist" => SheriffKey.Cultist,
                "Witch Doctor" => SheriffKey.Cultist,
                _ => SheriffKey.NotSuspicious
            }
        };
    }

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

    [TestCaseSource(typeof(DetectVulnerableCases))]
    public void DetectVulnerable(string rolesString, bool detects, SheriffKey result)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new SheriffSetup
        {
            DetectsArsonist = detects,
            DetectsCult = detects,
            DetectsMafiaTriad = detects,
            DetectsSerialKiller = detects,
            DetectsMassMurderer = detects
        });
        match.Start();

        var target = match.AllPlayers[1];
        target.Perks.DetectionImmune = false;
        target.Perks.CurrentlyDetectionImmune = false;

        Detect(match, result);
    }

    [TestCase("Sheriff,Godfather", true, true, SheriffKey.NotSuspicious)]
    [TestCase("Sheriff,Godfather", true, false, SheriffKey.Mafia)]
    [TestCase("Sheriff,Godfather", false, true, SheriffKey.NotSuspicious)]
    [TestCase("Sheriff,Godfather", false, false, SheriffKey.NotSuspicious)]
    public void DetectImmune(string rolesString, bool detects, bool immune, SheriffKey result)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new SheriffSetup
        {
            DetectsArsonist = detects,
            DetectsCult = detects,
            DetectsMafiaTriad = detects,
            DetectsSerialKiller = detects,
            DetectsMassMurderer = detects
        });

        match.Start();

        var target = match.AllPlayers[1];
        target.Perks.DetectionImmune = immune;
        target.Perks.CurrentlyDetectionImmune = immune;

        Detect(match, result);
    }
}

public class DetectVulnerableCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        // TODO: Change to RoleRegistry once all the abilities are done
        foreach (var role in RoleRegistry.Default.Entries())
        {
            if (!role.Natural) continue;

            var roleNames = $"Sheriff,{role.Name}";

            foreach (var detect in new[] {true, false})
            {
                var result = detect
                    ? SheriffTest.Key(role.Team.Id, role.Id)
                    : SheriffKey.NotSuspicious;
                yield return new object[] {roleNames, detect, result};
            }
        }
    }
}