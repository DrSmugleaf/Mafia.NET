using System.Globalization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using NUnit.Framework;

namespace Mafia.NET.IntegrationTests.Matches
{
    [TestFixture]
    [TestOf(typeof(Match))]
    public class MatchTest
    {
        [Test]
        public void NoActionMatch()
        {
            var roleNames =
                "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Mafioso,Mafioso,Mafioso"
                    .Split(",");
            var roles = roleNames.Length;
            var match = new Match(roleNames);
            match.Start();

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

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

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }
        }

        [Test]
        public void _93Match1Kill()
        {
            var roleNames =
                "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Godfather,Mafioso,Agent"
                    .Split(",");
            var roles = roleNames.Length;
            var match = new Match(roleNames);
            match.Start();

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

            var culture = new CultureInfo("en-US");
            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            match.Skip<NightPhase>();

            var gf = match.AllPlayers[9].Role.Ability;
            Assert.That(gf.Name, Is.EqualTo("Godfather"));
            
            foreach (var player in match.AllPlayers)
            {
                gf.TargetManager.Set(player);

                if (!player.Alive || player.Role.Team == gf.User.Role.Team)
                    Assert.That(gf.TargetManager[0], Is.Not.EqualTo(player));
                else Assert.That(gf.TargetManager[0], Is.EqualTo(player));
            }
            
            gf.TargetManager.Set(match.AllPlayers[0]);
            
            match.Skip();
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles - 1));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.EqualTo(1));

            match.Skip();
            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles - 1));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.EqualTo(1));

            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                
                if (player.Number != 1)
                    Assert.That(player.Alive, Is.True);
                else Assert.That(player.Alive, Is.False);
                
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }
            
            match.Skip();
            match.Skip();
            
            foreach (var living in match.LivingPlayers)
                Assert.That(living.Role.Ability.TargetManager[0], Is.Null);
            
            foreach (var dead in match.Graveyard.AllDeaths())
                Assert.That(dead.Victim.Role.Ability.TargetManager.Night().Targets, Is.Empty);
        }
    }
}