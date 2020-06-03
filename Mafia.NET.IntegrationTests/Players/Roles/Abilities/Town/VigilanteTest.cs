using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Vigilante))]
    public class VigilanteTest : BaseMatchTest
    {
        [TestCase("Vigilante,Citizen,Mafioso", true, false)]
        [TestCase("Vigilante,Citizen,Mafioso", false, true)]
        [TestCase("Vigilante,Godfather", true, true)]
        [TestCase("Vigilante,Godfather", false, true)]
        public void Kill(string rolesString, bool attack, bool survived)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var vigilante = match.AllPlayers[0];
            var other = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (attack) vigilante.Target(other);

            match.Skip<DeathsPhase>();

            Assert.That(vigilante.Alive, Is.True);
            Assert.That(other.Alive, Is.True);
            Deaths(match, 0);

            match.Skip<NightPhase>();

            if (attack) vigilante.Target(other);

            match.Skip<DeathsPhase>();

            Assert.That(vigilante.Alive, Is.True);
            Assert.That(other.Alive, Is.EqualTo(survived));
            Deaths(match, survived ? 0 : 1);
        }
    }
}