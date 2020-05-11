using System.Collections.Generic;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;
using NUnit.Framework;

namespace Mafia.NET.Tests.Matches
{
    [TestFixture]
    [TestOf(typeof(Match))]
    [Category("Long")]
    public class LongMatchTests
    {
        [Test]
        public void NoActionMatch()
        {
            var roleRegistry = RoleRegistry.Default;
            var roleNames =
                "Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Citizen,Mafioso,Mafioso,Mafioso"
                    .Split(",");
            var roles = roleRegistry.Get(roleNames);
            var roleSetup = new RoleSetup(roles);
            var setup = new Setup(roleSetup);
            var controllers = new List<IController>();
            for (var i = 0; i < 15; i++) controllers.Add(new Controller($"Bot {i + 1}"));

            var match = new Lobby(setup, controllers).Start();
            match.Start();

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roleNames.Length));
            Assert.That(match.Graveyard.AllDeaths(), Is.Empty);

            foreach (var player in match.AllPlayers)
            {
                Assert.That(player.Name, Is.EqualTo("Bot " + player.Id));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name, Is.EqualTo(roleNames[player.Id - 1]));
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
                Assert.That(player.Name, Is.EqualTo("Bot " + player.Id));
                Assert.That(player.Alive, Is.True);
                Assert.That(player.Role.Name, Is.EqualTo(roleNames[player.Id - 1]));
            }
        }
    }
}