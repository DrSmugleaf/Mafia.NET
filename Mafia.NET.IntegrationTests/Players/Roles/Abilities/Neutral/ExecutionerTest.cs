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
[TestOf(typeof(Obsession))]
public class ExecutionerTest : BaseMatchTest
{
    [TestCaseSource(typeof(LynchCases))]
    public void Lynch(string rolesString, bool lynch)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var executioner = match.AllPlayers[0];
        var ability = executioner.Abilities.Get<Obsession>()!;
        var target = ability.Target;

        if (lynch)
        {
            var accuse = match.Skip<AccusePhase>();

            foreach (var player in match.AllPlayers)
                accuse.AccuseManager.Accuse(player, target);

            var verdict = match.Skip<VerdictVotePhase>();

            foreach (var player in match.AllPlayers)
                verdict.Verdicts.AddVerdict(player, Verdict.Guilty);

            match.Skip<ExecutionRevealPhase>();
        }

        match.Skip<DiscussionPhase>();

        Assert.That(target.Alive, Is.EqualTo(!lynch));
        Assert.That(ability.Success, Is.EqualTo(lynch));
        Deaths(match, lynch ? 1 : 0);
    }
        
    public class LynchCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roleNames = "Executioner,Citizen,Citizen,Citizen";

            foreach (var lynch in new[] {true, false})
                yield return new object[] {roleNames, lynch};
        }
    }
}