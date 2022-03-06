using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral;

[TestFixture]
[TestOf(typeof(Arson))]
public class ArsonistTest : BaseMatchTest
{
    [TestCase("Arsonist,Investigator,Citizen", true, 2)]
    [TestCase("Arsonist,Investigator,Citizen", false, 1)]
    public void KillTargets(string rolesString, bool killsTargets, int deaths)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new ArsonSetup
        {
            IgnitionKillsVictimsTargets = killsTargets
        });
        match.Start();

        var arsonist = match.AllPlayers[0];
        var investigator = match.AllPlayers[1];
        var citizen = match.AllPlayers[2];

        match.Skip<NightPhase>();

        arsonist.Target(investigator);

        match.Skip<DeathsPhase>();

        Deaths(match, 0);

        match.Skip<NightPhase>();

        arsonist.Target(arsonist);
        investigator.Target(citizen);

        match.Skip<DeathsPhase>();

        Deaths(match, deaths);
    }

    [TestCase("Arsonist,Doctor,Citizen", true, 1)]
    [TestCase("Arsonist,Doctor,Citizen", false, 0)]
    public void AlwaysKill(string rolesString, bool alwaysKill, int deaths)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new ArsonSetup
        {
            IgnitionAlwaysKills = alwaysKill
        });
        match.Start();

        var arsonist = match.AllPlayers[0];
        var doctor = match.AllPlayers[1];
        var citizen = match.AllPlayers[2];

        match.Skip<NightPhase>();

        arsonist.Target(citizen);

        match.Skip<DeathsPhase>();

        Deaths(match, 0);

        match.Skip<NightPhase>();

        arsonist.Target(arsonist);
        doctor.Target(citizen);

        match.Skip<DeathsPhase>();

        Deaths(match, deaths);
    }

    [TestCase("Arsonist,Citizen", true)]
    [TestCase("Arsonist,Citizen", false)]
    public void VictimNotices(string rolesString, bool notices)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new ArsonSetup
        {
            VictimNoticesDousing = notices
        });
        match.Start();

        var arsonist = match.AllPlayers[0];
        var citizen = match.AllPlayers[1];

        match.Skip<NightPhase>();

        arsonist.Target(citizen);
        var notifications = new List<Text>();
        var personalMessage = Notification.Chat(arsonist.Role, ArsonKey.OtherDouse, citizen.Role).Localize();
        citizen.Chat += (s, e) =>
        {
            if (Equals(e, personalMessage)) notifications.Add(e);
        };

        match.Skip<DeathsPhase>();

        Assert.That(notifications, notices ? Is.Not.Empty : Is.Empty);
    }

    [TestCase("Arsonist,Escort,Citizen", true)]
    [TestCase("Arsonist,Escort,Citizen", false)]
    public void DousesBlockers(string rolesString, bool dousesBlockers)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new ArsonSetup
        {
            DousesRoleBlockers = dousesBlockers
        });
        match.Start();

        var arsonist = match.AllPlayers[0];
        var escort = match.AllPlayers[1];
        var citizen = match.AllPlayers[2];

        match.Skip<NightPhase>();

        arsonist.Target(citizen);
        escort.Target(arsonist);

        match.Skip<DeathsPhase>();

        Assert.That(escort.Perks.Doused, Is.EqualTo(dousesBlockers));
        Assert.That(citizen.Perks.Doused, Is.False);
    }

    [TestCase("Arsonist,Bodyguard,Citizen", true, 1)]
    [TestCase("Arsonist,Bodyguard,Citizen", false, 1)]
    public void Guarded(string rolesString, bool guarded, int deaths)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.Start();

        var arsonist = match.AllPlayers[0];
        var bodyguard = match.AllPlayers[1];
        var citizen = match.AllPlayers[2];

        match.Skip<NightPhase>();

        arsonist.Target(citizen);

        match.Skip<DeathsPhase>();
        match.Skip<NightPhase>();

        arsonist.Target(arsonist);
        bodyguard.Target(citizen);

        match.Skip<DeathsPhase>();

        Assert.That(arsonist.Alive, Is.True);
        Assert.That(bodyguard.Alive, Is.True);
        Assert.That(citizen.Alive, Is.False);
        Deaths(match, 1);
    }
}