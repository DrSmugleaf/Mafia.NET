using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(Vigilante))]
    public class SerialKillerTest : BaseMatchTest
    {
        [TestCase("Serial Killer,Citizen", true, false)]
        [TestCase("Serial Killer,Citizen", false, true)]
        [TestCase("Serial Killer,Godfather", true, true)]
        [TestCase("Serial Killer,Godfather", false, true)]
        public void Kill(string namesString, bool attack, bool survived)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var sk = match.AllPlayers[0];
            var other = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (attack) sk.Role.Ability.TargetManager.Set(other);

            match.Skip<DeathsPhase>();

            Assert.That(sk.Alive, Is.True);
            Assert.That(other.Alive, Is.EqualTo(survived));
            Deaths(match, survived ? 0 : 1);
        }
    }
}