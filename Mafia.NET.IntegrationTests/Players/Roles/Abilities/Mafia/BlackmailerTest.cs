using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Blackmail))]
    public class BlackmailerTest : BaseMatchTest
    {
        [TestCaseSource(typeof(BlackmailerCases))]
        public void Blackmail(string rolesString, bool blackmail, bool talkDuringTrial)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Replace(new BlackmailSetup
            {
                TalkDuringTrial = talkDuringTrial
            }, new MafiaMinionSetup
            {
                BecomesHenchmanIfAlone = false
            });
            match.Start();

            var blackmailer = match.AllPlayers[0];
            var target = match.AllPlayers[1];

            match.Skip<DiscussionPhase>();

            var chat = match.Chat.Main();

            var sent = chat.TrySend(target, "1", out var message);
            Assert.That(sent, Is.True);
            Assert.That(message, Is.Not.Null);

            match.Skip<AccusePhase>();

            sent = chat.TrySend(target, "2", out message);
            Assert.That(sent, Is.True);
            Assert.That(message, Is.Not.Null);

            match.Skip<NightPhase>();

            Assert.That(target.Perks.Blackmailed, Is.False);
            if (blackmail) blackmailer.Target(target);

            match.Skip<DeathsPhase>();

            Assert.That(target.Perks.Blackmailed, Is.EqualTo(blackmail));

            match.Skip<DiscussionPhase>();

            sent = chat.TrySend(target, "3", out message);
            Assert.That(sent, Is.EqualTo(!blackmail));
            Assert.That(message, blackmail ? Is.Null : Is.Not.Null);

            var accuse = match.Skip<AccusePhase>();

            sent = chat.TrySend(target, "4", out message);
            Assert.That(sent, Is.EqualTo(!blackmail));
            Assert.That(message, blackmail ? Is.Null : Is.Not.Null);

            sent = chat.TrySend(blackmailer, "5", out message);
            Assert.That(sent, Is.True);
            Assert.That(message, Is.Not.Null);

            foreach (var player in match.AllPlayers)
                accuse.AccuseManager.Accuse(player, target);

            match.Skip<DefensePhase>();

            sent = chat.TrySend(blackmailer, "6", out message);
            Assert.That(sent, Is.False);
            Assert.That(message, Is.Null);

            sent = chat.TrySend(target, "7", out message);
            Assert.That(sent, Is.EqualTo(!blackmail || talkDuringTrial));
            Assert.That(message, !blackmail || talkDuringTrial ? Is.Not.Null : Is.Null);

            var verdict = match.Skip<VerdictVotePhase>();

            sent = chat.TrySend(target, "8", out message);
            Assert.That(sent, Is.EqualTo(!blackmail || talkDuringTrial));
            Assert.That(message, !blackmail || talkDuringTrial ? Is.Not.Null : Is.Null);

            foreach (var player in match.AllPlayers)
                verdict.Verdicts.AddVerdict(player, Verdict.Guilty);

            match.Skip<LastWordsPhase>();

            sent = chat.TrySend(target, "9", out message);
            Assert.That(sent, Is.True);
            Assert.That(message, Is.Not.Null);
        }
    }

    public class BlackmailerCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var blackmailers = new[] {"Blackmailer", "Silencer"};

            foreach (var blackmailer in blackmailers)
            {
                var roleNames = $"{blackmailer},Citizen,Citizen";

                foreach (var blackmail in new[] {true, false})
                foreach (var talkDuringTrial in new[] {true, false})
                    yield return new object[] {roleNames, blackmail, talkDuringTrial};
            }
        }
    }
}