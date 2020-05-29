using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Escort))]
    public class EscortTest : BaseMatchTest
    {
        [TestCase("Escort,Citizen,Mafioso", true, true)]
        [TestCase("Escort,Citizen,Mafioso", false, false)]
        [TestCase("Escort,Citizen,Godfather", true, true)]
        [TestCase("Escort,Citizen,Godfather", false, false)]
        public void Block(string namesString, bool blocked, bool alive)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var escort = match.AllPlayers[0];
            var citizen = match.AllPlayers[1];
            var attacker = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (blocked) escort.Target(attacker);
            attacker.Target(citizen);

            match.Skip<DeathsPhase>();

            Deaths(match, alive ? 0 : 1);
        }
    }
}