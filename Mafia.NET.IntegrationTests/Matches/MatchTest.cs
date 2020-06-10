using System.Globalization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Matches
{
    [TestFixture]
    [TestOf(typeof(Match))]
    public class MatchTest : BaseMatchTest
    {
        [TestCase(
            "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Mafioso,Mafioso,Mafioso")]
        public void NoActionMatch(string rolesString)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            Deaths(match, 0);

            var culture = new CultureInfo("en-US");
            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            var phases = new[]
            {
                typeof(PresentationPhase),
                typeof(DiscussionPhase),
                typeof(AccusePhase),
                typeof(NightPhase),
                typeof(DeathsPhase),
                typeof(DiscussionPhase)
            };

            foreach (var phase in phases)
            {
                Assert.That(match.Phase.CurrentPhase, Is.TypeOf(phase));
                match.Skip();
            }

            Deaths(match, 0);

            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }
        }

        [TestCase("Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Godfather,Mafioso,Agent")]
        public void OneKill(string rolesString)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var citizen = match.AllPlayers[0];
            var gf = match.AllPlayers[9];

            Deaths(match, 0);

            var culture = new CultureInfo("en-US");
            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            match.Skip<NightPhase>();

            Assert.That(gf.Role.Id, Is.EqualTo("Godfather"));

            foreach (var player in match.AllPlayers)
            {
                gf.Target(player);

                if (!player.Alive || player.Role.Team == gf.Role.Team)
                    Assert.That(gf.Targets[0], Is.Not.EqualTo(player));
                else Assert.That(gf.Targets[0], Is.EqualTo(player));
            }

            gf.Target(citizen);

            match.Skip<DeathsPhase>();
            Deaths(match, 1);

            match.Skip<DiscussionPhase>();

            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));

                if (player.Number != 1)
                    Assert.That(player.Alive, Is.True);
                else Assert.That(player.Alive, Is.False);

                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            match.Skip<DeathsPhase>();

            foreach (var living in match.LivingPlayers)
                Assert.That(living.Targets[0], Is.Null);
        }
    }
}