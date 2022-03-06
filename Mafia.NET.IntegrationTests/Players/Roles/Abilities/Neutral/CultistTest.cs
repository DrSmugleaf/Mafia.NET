using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral;

[TestFixture]
[TestOf(typeof(CultChat))]
[TestOf(typeof(CultSuggest))]
public class CultistTest : BaseMatchTest
{
    [TestCaseSource(typeof(SuggestCases))]
    public void ConvertImmune(string rolesString, bool convert, bool immunityPrevents)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new CultSuggestSetup
        {
            ImmunityPreventsConversion = immunityPrevents
        });
        match.Start();

        var cultist = match.AllPlayers[0];
        var target = match.AllPlayers[1];

        match.Skip<NightPhase>();

        if (convert) cultist.Target(target);

        match.Skip<DeathsPhase>();

        Assert.That(target.Role.Id, Is.EqualTo(convert && !immunityPrevents ? "Cultist" : roleNames[1]));

        Deaths(match, 0);
    }

    [TestCaseSource(typeof(ChatCases))]
    public void Chat(string rolesString)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var first = match.AllPlayers[0];
        var second = match.AllPlayers[1];

        match.Skip<NightPhase>();

        var firstText = "Did you ever hear the tragedy of Darth Plagueis The Wise?";
        var firstMessages = match.Chat.Send(first, firstText);

        var secondText = "I thought not. It's not a story the Jedi would tell you.";
        var secondMessages = match.Chat.Send(second, secondText);

        Messages(firstMessages, 1, first, firstText, null, first, second);
        Messages(secondMessages, 1, second, secondText, null, first, second);

        match.Skip<DeathsPhase>();

        Deaths(match, 0);
    }
}

public class SuggestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var roleNames = "Cultist,Serial Killer,Citizen,Citizen,Citizen,Citizen";

        foreach (var convert in new[] {true, false})
        foreach (var immunityPrevents in new[] {true, false})
            yield return new object[] {roleNames, convert, immunityPrevents};
    }
}

public class ChatCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var roleNames = "Cultist,Cultist,Citizen";
        yield return new object[] {roleNames};
    }
}