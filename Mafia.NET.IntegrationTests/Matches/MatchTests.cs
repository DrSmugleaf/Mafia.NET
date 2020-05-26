using System;
using System.Globalization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Selectors;
using NUnit.Framework;

namespace Mafia.NET.IntegrationTests.Matches
{
    [TestFixture]
    [TestOf(typeof(Match))]
    public class MatchTests
    {
        [Test]
        public void NoActionMatch()
        {
            var roleRegistry = RoleRegistry.Default;
            var roleNames =
                "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Mafioso,Mafioso,Mafioso"
                    .Split(",");
            var roles = roleRegistry.Selectors(roleNames);
            var roleSetup = new RoleSetup(roles);
            var setup = new Setup(roleSetup);
            var hostName = "Bot 1";
            var lobby = new Lobby(Guid.NewGuid(), hostName, Guid.NewGuid(), setup);
            for (var i = 1; i < 15; i++) lobby.Add($"Bot {i + 1}", Guid.NewGuid());

            Assert.That(lobby.Host.Name, Is.EqualTo(hostName));

            var match = lobby.Start();
            match.Start();

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

            var culture = new CultureInfo("en-US");
            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<PresentationPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DiscussionPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<AccusePhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<NightPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DeathsPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DiscussionPhase>());

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roleNames.Length));
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
            var roleRegistry = RoleRegistry.Default;
            var roleNames =
                "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Godfather,Mafioso,Agent"
                    .Split(",");
            var roles = roleRegistry.Selectors(roleNames);
            var roleSetup = new RoleSetup(roles);
            var setup = new Setup(roleSetup);
            var hostName = "Bot 1";
            var lobby = new Lobby(Guid.NewGuid(), hostName, Guid.NewGuid(), setup);
            for (var i = 1; i < 12; i++) lobby.Add($"Bot {i + 1}", Guid.NewGuid());

            Assert.That(lobby.Host.Name, Is.EqualTo(hostName));

            var match = lobby.Start();
            match.Start();

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

            var culture = new CultureInfo("en-US");
            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name.String, Is.EqualTo("Bot " + player.Number));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name.Localize(culture).ToString(), Is.EqualTo(roleNames[player.Number - 1]));
            }

            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<PresentationPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DiscussionPhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<AccusePhase>());
            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<NightPhase>());

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
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DeathsPhase>());
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles.Count));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles.Count - 1));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.EqualTo(1));

            match.Skip();
            Assert.That(match.Phase.CurrentPhase, Is.TypeOf<DiscussionPhase>());

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roleNames.Length - 1));
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