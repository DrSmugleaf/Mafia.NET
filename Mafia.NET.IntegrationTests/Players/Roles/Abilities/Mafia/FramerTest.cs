using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia;

[TestFixture]
[TestOf(typeof(Frame))]
public class FramerTest : BaseMatchTest
{
    [TestCaseSource(typeof(FrameCases))]
    public void Frame(string rolesString, bool frame)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new MafiaMinionSetup
        {
            BecomesHenchmanIfAlone = false
        });
        match.Start();

        var framer = match.AllPlayers[0];
        var target = match.AllPlayers[1];
        var investigator = match.AllPlayers[2];
        var sheriff = match.AllPlayers[3];

        match.Skip<NightPhase>();

        if (frame) framer.Target(target);
        investigator.Target(target);
        sheriff.Target(target);

        var investigations = new List<Text>();
        var interrogations = new List<Text>();
        investigator.Chat += (s, e) => investigations.Add(e);
        sheriff.Chat += (s, e) => interrogations.Add(e);

        match.Skip<DeathsPhase>();

        Assert.That(investigations.Count, Is.Positive);
        Assert.That(interrogations.Count, Is.Positive);

        Assert.That(investigations[0].ToString(), frame
            ? Does.Contain("guilty")
            : Does.Not.Contain("guilty"));
        Assert.That(investigations[0].ToString(), frame
            ? Does.Not.Contain("innocent")
            : Does.Contain("innocent"));

        var interrogation = new Key(frame
            ? framer.Role.Team.Id == "Mafia" ? SheriffKey.Mafia : SheriffKey.Triad
            : SheriffKey.NotSuspicious).Localize();
        Assert.That(interrogations[0], Is.EqualTo(interrogation));
    }
}

public class FrameCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var framers = new[] {"Framer", "Forger"};

        foreach (var framer in framers)
        {
            var roleNames = $"{framer},Citizen,Investigator,Sheriff";

            foreach (var block in new[] {true, false})
                yield return new object[] {roleNames, block};
        }
    }
}