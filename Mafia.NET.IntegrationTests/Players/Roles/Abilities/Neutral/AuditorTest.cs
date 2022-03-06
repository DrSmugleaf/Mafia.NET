using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral;

[TestFixture]
[TestOf(typeof(Audit))]
public class AuditorTest : BaseMatchTest
{
    [TestCase("Auditor,Doctor,Citizen", true, "Citizen")]
    [TestCase("Auditor,Doctor,Citizen", false, "Citizen")]
    public void AuditVulnerable(string rolesString, bool audit, string auditedRole)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var auditor = match.AllPlayers[0];
        var target = match.AllPlayers[1];

        match.Skip<NightPhase>();

        if (audit) auditor.Target(target);

        var ability = auditor.Abilities.Get<Audit>()!;
        Assert.NotNull(ability);

        var uses = ability.Uses;

        match.Skip<DeathsPhase>();

        Assert.That(auditor.Role.Id, Is.EqualTo(roleNames[0]));

        if (audit)
        {
            Assert.AreNotEqual(target.Role.Id, roleNames[1]);
            Assert.That(target.Role.Id, Is.EqualTo(auditedRole));
            Assert.That(ability.Uses, Is.EqualTo(uses - 1));
        }
        else
        {
            Assert.That(target.Role.Id, Is.EqualTo(roleNames[1]));
            Assert.That(ability.Uses, Is.EqualTo(uses));
        }

        Deaths(match, 0);
    }

    [TestCase("Auditor,Godfather,Citizen", true)]
    [TestCase("Auditor,Godfather,Citizen", false)]
    public void AuditImmune(string rolesString, bool audit)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var auditor = match.AllPlayers[0];
        var target = match.AllPlayers[1];

        match.Skip<NightPhase>();

        if (audit) auditor.Target(target);

        var ability = auditor.Abilities.Get<Audit>()!;
        Assert.NotNull(ability);

        var uses = ability.Uses;

        match.Skip<DeathsPhase>();

        Assert.That(auditor.Role.Id, Is.EqualTo(roleNames[0]));
        Assert.That(target.Role.Id, Is.EqualTo(roleNames[1]));
        Assert.That(ability.Uses, Is.EqualTo(uses));

        Deaths(match, 0);
    }
}