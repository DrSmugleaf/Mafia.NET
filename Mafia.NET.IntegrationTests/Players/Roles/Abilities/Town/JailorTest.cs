using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Jailor))]
    public class JailorTest : BaseMatchTest
    {
        [TestCase("Jailor,Citizen,Citizen,Citizen", true, true)]
        [TestCase("Jailor,Citizen,Citizen,Citizen", true, false)]
        [TestCase("Jailor,Citizen,Citizen,Citizen", false, false)]
        [TestCase("Jailor,Citizen,Citizen,Citizen", false, true)]
        [TestCase("Jailor,Mafioso,Mafioso,Mafioso", true, true)]
        [TestCase("Jailor,Mafioso,Mafioso,Mafioso", true, false)]
        [TestCase("Jailor,Mafioso,Mafioso,Mafioso", false, false)]
        [TestCase("Jailor,Mafioso,Mafioso,Mafioso", false, true)]
        public void Execute(string namesString, bool lynch, bool execute)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var jailor = match.AllPlayers[0];
            var prisoner = match.AllPlayers[1];
            var lynched = match.AllPlayers[2];

            match.Skip<DiscussionPhase>();

            jailor.Role.Ability.TargetManager.Set(prisoner);

            if (lynch)
            {
                var accuse = match.Skip<AccusePhase>();

                foreach (var player in match.AllPlayers)
                    accuse.AccuseManager.Accuse(player, lynched);

                var verdict = match.Skip<VerdictVotePhase>();

                foreach (var player in match.AllPlayers)
                    verdict.Verdicts.AddVerdict(player, Verdict.Guilty);

                match.Skip<ExecutionRevealPhase>();
            }

            Assert.That(lynched.Alive, Is.EqualTo(!lynch));

            match.Skip<NightPhase>();

            if (execute) jailor.Role.Ability.TargetManager.Set(prisoner);

            if (lynch)
            {
                Assert.That(prisoner.Role.Ability.Active, Is.True);
                Assert.That(jailor.Role.Ability.TargetManager.Day(), Is.Null);
                Assert.That(jailor.Role.Ability.TargetManager.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Role.Ability.Active, Is.False);
                Assert.That(jailor.Role.Ability.TargetManager.Day(), Is.Not.Null);
                Assert.That(jailor.Role.Ability.TargetManager.Night(), execute ? Is.Not.Null : Is.Null);
            }

            match.Skip<DeathsPhase>();

            Deaths(match, lynch || execute ? 1 : 0);
        }

        [TestCase("Jailor,Citizen,Citizen,Citizen", true)]
        [TestCase("Jailor,Citizen,Citizen,Citizen", false)]
        public void NightChat(string namesString, bool lynch)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var jailor = match.AllPlayers[0];
            var prisoner = match.AllPlayers[1];
            var lynched = match.AllPlayers[2];

            match.Skip<DiscussionPhase>();

            jailor.Role.Ability.TargetManager.Set(prisoner);

            if (lynch)
            {
                var accuse = match.Skip<AccusePhase>();

                foreach (var player in match.AllPlayers)
                    accuse.AccuseManager.Accuse(player, lynched);

                var verdict = match.Skip<VerdictVotePhase>();

                foreach (var player in match.AllPlayers)
                    verdict.Verdicts.AddVerdict(player, Verdict.Guilty);

                match.Skip<ExecutionRevealPhase>();
            }

            Assert.That(lynched.Alive, Is.EqualTo(!lynch));

            match.Skip<NightPhase>();

            if (lynch)
            {
                Assert.That(prisoner.Role.Ability.Active, Is.True);
                Assert.That(jailor.Role.Ability.TargetManager.Day(), Is.Null);
                Assert.That(jailor.Role.Ability.TargetManager.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Role.Ability.Active, Is.False);
                Assert.That(jailor.Role.Ability.TargetManager.Day(), Is.Not.Null);
                Assert.That(jailor.Role.Ability.TargetManager.Night(), Is.Null);
            }

            var text = "Did you ever hear the tragedy of Darth Plagueis The Wise?";
            var messages = match.Chat.Send(jailor, text);
            var nickname = new Key(JailorKey.Nickname);

            Messages(messages, lynch ? 0 : 1, jailor, text, nickname, jailor, prisoner);

            match.Skip<DeathsPhase>();

            Deaths(match, lynch ? 1 : 0);
        }
    }
}