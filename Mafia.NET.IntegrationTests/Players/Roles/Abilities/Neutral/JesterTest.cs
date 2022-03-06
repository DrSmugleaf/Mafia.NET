using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral;

[TestFixture]
[TestOf(typeof(Annoy))]
[TestOf(typeof(Jester))]
public class JesterTest : BaseMatchTest
{
    [TestCaseSource(typeof(LynchCases))]
    public void Lynch(string rolesString, bool lynch, bool dies)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new JesterSetup
        {
            RandomGuiltyVoterDies = dies
        });
        match.Start();

        var jester = match.AllPlayers[0];

        if (lynch)
        {
            var accuse = match.Skip<AccusePhase>();

            foreach (var player in match.AllPlayers)
                accuse.AccuseManager.Accuse(player, jester);

            var verdict = match.Skip<VerdictVotePhase>();

            foreach (var player in match.AllPlayers)
                verdict.Verdicts.AddVerdict(player, Verdict.Guilty);

            match.Skip<ExecutionRevealPhase>();
        }

        match.Skip<DeathsPhase>();

        Assert.That(jester.Alive, Is.EqualTo(!lynch));
        Deaths(match, lynch && dies ? 2 : lynch ? 1 : 0);
    }
}

public class LynchCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var roleNames = "Jester,Serial Killer,Mafioso,Citizen";

        foreach (var lynch in new[] {true, false})
        foreach (var dies in new[] {true, false})
            yield return new object[] {roleNames, lynch, dies};
    }
}