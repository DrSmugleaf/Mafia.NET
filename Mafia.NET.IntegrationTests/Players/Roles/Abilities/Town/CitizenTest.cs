using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town;

[TestFixture]
[TestOf(typeof(Vest))]
public class CitizenTest : BaseMatchTest
{
    [TestCase("Citizen,Mafioso", true)]
    [TestCase("Citizen,Mafioso", false)]
    public void Vest(string rolesString, bool vest)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var citizen = match.AllPlayers[0];
        var attacker = match.AllPlayers[1];

        match.Skip<NightPhase>();

        if (vest) citizen.Target(citizen);
        attacker.Target(citizen);

        match.Skip<DeathsPhase>();

        Assert.That(citizen.Alive, Is.EqualTo(vest));
        Assert.That(attacker.Alive, Is.True);
    }
}