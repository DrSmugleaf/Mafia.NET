using Mafia.NET.Matches;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Matches
{
    public abstract class BaseMatchTest
    {
        public void Deaths(IMatch match, int deaths)
        {
            var roles = match.Setup.Roles.Selectors.Count;
            
            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles - deaths));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.EqualTo(deaths));
        }
    }
}