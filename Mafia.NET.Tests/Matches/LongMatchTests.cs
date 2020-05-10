using System.Collections.Generic;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Options;
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
        public void Match()
        {
            var roleRegistry = RoleRegistry.Default;
            var roles = roleRegistry.Get(
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Citizen",
                "Mafioso",
                "Mafioso",
                "Mafioso");
            var roleSetup = new RoleSetup(roles);
            var setup = new Setup(roleSetup);
            var controllers = new List<IController>();
            for (var i = 0; i < 15; i++) controllers.Add(new Controller($"Bot {i + 1}"));

            var match = new Match(setup, controllers);
            match.Start();
        }
    }
}