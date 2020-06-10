using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Alert))]
    public class VeteranTest : BaseMatchTest
    {
        [TestCase("Veteran,Mafioso", true, true, true, 1)]
        [TestCase("Veteran,Mafioso", true, false, true, 0)]
        [TestCase("Veteran,Mafioso", false, true, false, 1)]
        [TestCase("Veteran,Mafioso", false, false, true, 0)]
        [TestCase("Veteran,Mafioso,Sheriff,Investigator", true, true, true, 3)]
        [TestCase("Veteran,Lookout,Amnesiac,Coroner,Janitor,Mafioso", true, true, true, 1)]
        public void Alert(string rolesString, bool alert, bool attack, bool survived, int deaths)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var veteran = match.AllPlayers[0];

            match.Skip<NightPhase>();

            if (alert) veteran.Target(veteran);

            if (attack)
            {
                var living = match.LivingPlayers;

                for (var i = 1; i < living.Count; i++)
                {
                    var attacker = living[i];
                    attacker.Target(veteran);
                }
            }

            match.Skip<DeathsPhase>();

            Assert.That(veteran.Alive, survived ? (IResolveConstraint) Is.True : Is.False);
            Deaths(match, deaths);
        }
    }
}