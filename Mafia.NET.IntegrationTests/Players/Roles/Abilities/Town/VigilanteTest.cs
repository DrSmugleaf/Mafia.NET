using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Vigilante))]
    public class VigilanteTest : BaseMatchTest
    {
        [TestCase("Vigilante,Citizen", true, false)]
        [TestCase("Vigilante,Citizen", false, true)]
        [TestCase("Vigilante,Godfather", true, true)]
        [TestCase("Vigilante,Godfather", false, true)]
        public void Kill(string namesString, bool attack, bool survived)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var vigilante = match.AllPlayers[0];
            var other = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (attack) vigilante.Role.Ability.TargetManager.Set(other);

            match.Skip<DeathsPhase>();

            Assert.That(vigilante.Alive, Is.True);
            Assert.That(other.Alive, Is.EqualTo(survived));
            Deaths(match, survived ? 0 : 1);
        }
    }
}