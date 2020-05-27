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

            var uses = jailor.Role.Ability.Uses;

            match.Skip<DeathsPhase>();

            Deaths(match, lynch || execute ? 1 : 0);

            if (!lynch && execute)
                Assert.That(jailor.Role.Ability.Uses, Is.EqualTo(uses - 1));
            else Assert.That(jailor.Role.Ability.Uses, Is.EqualTo(uses));
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

        [TestCase("Jailor,Jailor,Citizen,Citizen", true, true)]
        [TestCase("Jailor,Jailor,Citizen,Citizen", true, false)]
        [TestCase("Jailor,Jailor,Citizen,Citizen", false, true)]
        [TestCase("Jailor,Jailor,Citizen,Citizen", false, false)]
        public void Multiple(string namesString, bool lynch, bool execute)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var first = match.AllPlayers[0];
            var second = match.AllPlayers[1];
            var prisoner = match.AllPlayers[2];
            var lynched = match.AllPlayers[3];

            match.Skip<DiscussionPhase>();

            first.Role.Ability.TargetManager.Set(prisoner);
            second.Role.Ability.TargetManager.Set(prisoner);

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

            if (execute)
            {
                first.Role.Ability.TargetManager.Set(prisoner);
                second.Role.Ability.TargetManager.Set(prisoner);
            }

            if (lynch)
            {
                Assert.That(prisoner.Role.Ability.Active, Is.True);
                Assert.That(first.Role.Ability.TargetManager.Day(), Is.Null);
                Assert.That(first.Role.Ability.TargetManager.Night(), Is.Null);
                Assert.That(second.Role.Ability.TargetManager.Day(), Is.Null);
                Assert.That(second.Role.Ability.TargetManager.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Role.Ability.Active, Is.False);
                Assert.That(first.Role.Ability.TargetManager.Day(), Is.Not.Null);
                Assert.That(first.Role.Ability.TargetManager.Night(), execute ? Is.Not.Null : Is.Null);
                Assert.That(second.Role.Ability.TargetManager.Day(), Is.Not.Null);
                Assert.That(second.Role.Ability.TargetManager.Night(), execute ? Is.Not.Null : Is.Null);
            }

            var firstText = "Did you ever hear the tragedy of Darth Plagueis The Wise?";
            var firstMessages = match.Chat.Send(first, firstText);

            var secondText = "I thought not. It's not a story the Jedi would tell you.";
            var secondMessages = match.Chat.Send(second, secondText);

            var nickname = new Key(JailorKey.Nickname);

            Messages(firstMessages, lynch ? 0 : 1, first, firstText, nickname, first, second, prisoner);
            Messages(secondMessages, lynch ? 0 : 1, second, secondText, nickname, first, second, prisoner);

            var firstUses = first.Role.Ability.Uses;
            var secondUses = second.Role.Ability.Uses;

            match.Skip<DeathsPhase>();

            Deaths(match, lynch || execute ? 1 : 0);

            if (!lynch && execute)
            {
                Assert.That(first.Role.Ability.Uses, Is.EqualTo(firstUses - 1));
                Assert.That(second.Role.Ability.Uses, Is.EqualTo(secondUses - 1));
            }
            else
            {
                Assert.That(first.Role.Ability.Uses, Is.EqualTo(firstUses));
                Assert.That(second.Role.Ability.Uses, Is.EqualTo(secondUses));
            }
        }
    }
}