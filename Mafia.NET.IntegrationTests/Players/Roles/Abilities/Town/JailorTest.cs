using System;
using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Actions;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Detain))]
    [TestOf(typeof(Jail))]
    [TestOf(typeof(Kidnap))]
    [TestOf(typeof(Execute))]
    public class JailorTest : BaseMatchTest
    {
        [TestCaseSource(typeof(ExecuteCases))]
        public void Execute(string rolesString, bool lynch, bool execute, Type ability)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var jailor = match.AllPlayers[0];
            var prisoner = match.AllPlayers[1];
            var lynched = match.AllPlayers[2];

            match.Skip<DiscussionPhase>();

            jailor.Target(prisoner);

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

            if (execute) jailor.Target(prisoner);

            if (lynch)
            {
                Assert.That(prisoner.Perks.RoleBlocked, Is.False);
                Assert.That(jailor.Targets.Day(), Is.Null);
                Assert.That(jailor.Targets.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Perks.RoleBlocked, Is.True);
                Assert.That(jailor.Targets.Day(), Is.Not.Null);
                Assert.That(jailor.Targets.Night(), execute ? Is.Not.Null : Is.Null);
            }

            var abilities = jailor.Abilities;
            var uses = abilities.Get(ability).Uses; // TODO

            match.Skip<DeathsPhase>();

            Deaths(match, lynch || execute ? 1 : 0);

            var newUses = abilities.Get(ability).Uses;
            if (!lynch && execute)
                Assert.That(newUses, Is.EqualTo(uses - 1));
            else Assert.That(newUses, Is.EqualTo(uses));
        }

        [TestCase("Jailor,Citizen,Citizen,Citizen", true)]
        [TestCase("Jailor,Citizen,Citizen,Citizen", false)]
        public void NightChat(string rolesString, bool lynch)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var jailor = match.AllPlayers[0];
            var prisoner = match.AllPlayers[1];
            var lynched = match.AllPlayers[2];

            match.Skip<DiscussionPhase>();

            jailor.Target(prisoner);

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
                Assert.That(prisoner.Perks.RoleBlocked, Is.False);
                Assert.That(jailor.Targets.Day(), Is.Null);
                Assert.That(jailor.Targets.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Perks.RoleBlocked, Is.True);
                Assert.That(jailor.Targets.Day(), Is.Not.Null);
                Assert.That(jailor.Targets.Night(), Is.Null);
            }

            var text = "Did you ever hear the tragedy of Darth Plagueis The Wise?";
            var messages = match.Chat.Send(jailor, text);
            var nickname = new Key(jailor.Role, DetainKey.Nickname);

            Messages(messages, lynch ? 0 : 1, jailor, text, nickname, jailor, prisoner);

            match.Skip<DeathsPhase>();

            Deaths(match, lynch ? 1 : 0);
        }

        [TestCase("Jailor,Jailor,Citizen,Citizen", true, true, typeof(Jail))]
        [TestCase("Jailor,Jailor,Citizen,Citizen", true, false, typeof(Jail))]
        [TestCase("Jailor,Jailor,Citizen,Citizen", false, true, typeof(Jail))]
        [TestCase("Jailor,Jailor,Citizen,Citizen", false, false, typeof(Jail))]
        public void Multiple(string rolesString, bool lynch, bool execute, Type ability)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var first = match.AllPlayers[0];
            var second = match.AllPlayers[1];
            var prisoner = match.AllPlayers[2];
            var lynched = match.AllPlayers[3];

            match.Skip<DiscussionPhase>();

            first.Target(prisoner);
            second.Target(prisoner);

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
                first.Target(prisoner);
                second.Target(prisoner);
            }

            if (lynch)
            {
                Assert.That(prisoner.Perks.RoleBlocked, Is.False);
                Assert.That(first.Targets.Day(), Is.Null);
                Assert.That(first.Targets.Night(), Is.Null);
                Assert.That(second.Targets.Day(), Is.Null);
                Assert.That(second.Targets.Night(), Is.Null);
            }
            else
            {
                Assert.That(prisoner.Perks.RoleBlocked, Is.True);
                Assert.That(first.Targets.Day(), Is.Not.Null);
                Assert.That(first.Targets.Night(), execute ? Is.Not.Null : Is.Null);
                Assert.That(second.Targets.Day(), Is.Not.Null);
                Assert.That(second.Targets.Night(), execute ? Is.Not.Null : Is.Null);
            }

            var firstText = "Did you ever hear the tragedy of Darth Plagueis The Wise?";
            var firstMessages = match.Chat.Send(first, firstText);

            var secondText = "I thought not. It's not a story the Jedi would tell you.";
            var secondMessages = match.Chat.Send(second, secondText);

            var nickname = new Key(first.Role, DetainKey.Nickname);

            Messages(firstMessages, lynch ? 0 : 1, first, firstText, nickname, first, second, prisoner);
            Messages(secondMessages, lynch ? 0 : 1, second, secondText, nickname, first, second, prisoner);

            var firstUses = first.Abilities.Get(ability).Uses;
            var secondUses = second.Abilities.Get(ability).Uses;

            match.Skip<DeathsPhase>();

            Deaths(match, lynch || execute ? 1 : 0);

            var firstNewUses = first.Abilities.Get(ability).Uses;
            var secondNewUses = second.Abilities.Get(ability).Uses;
            if (!lynch && execute)
            {
                Assert.That(firstNewUses, Is.EqualTo(firstUses - 1));
                Assert.That(secondNewUses, Is.EqualTo(secondUses - 1));
            }
            else
            {
                Assert.That(firstNewUses, Is.EqualTo(firstUses));
                Assert.That(secondNewUses, Is.EqualTo(secondUses));
            }
        }
    }

    public class ExecuteCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var jailors = new Dictionary<string, Type>
            {
                ["Jailor"] = typeof(Jail),
                ["Kidnapper"] = typeof(Kidnap)
            };

            foreach (var jailor in jailors)
            {
                var roleNames = $"{jailor.Key},Citizen,Citizen,Mafioso";

                foreach (var lynch in new[] {true, false})
                foreach (var execute in new[] {true, false})
                    yield return new object[] {roleNames, lynch, execute, jailor.Value};
            }
        }
    }
}