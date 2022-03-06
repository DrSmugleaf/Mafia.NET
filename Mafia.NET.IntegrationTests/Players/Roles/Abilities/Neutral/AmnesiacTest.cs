using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Perks;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral;

[TestFixture]
[TestOf(typeof(Remember))]
public class AmnesiacTest : BaseMatchTest
{
    private bool Successful(
        IPlayer victim,
        bool remember,
        bool town,
        bool mafia,
        bool killing)
    {
        var killingRole = victim.Role.Categories
            .Any(category => category.Id.Contains("Killing"));

        return !victim.Alive &&
               !victim.Role.Unique &&
               remember &&
               (!killingRole || killing) &&
               victim.Role.Team.Id switch
               {
                   "Town" => town,
                   "Mafia" => mafia,
                   "Triad" => mafia,
                   _ => true
               };
    }

    [TestCaseSource(typeof(RememberCases))]
    public void Remember(
        string rolesString,
        bool attack,
        bool remember,
        bool announce,
        bool town,
        bool mafia,
        bool killing)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new RememberSetup
        {
            CanBecomeTown = town,
            CanBecomeMafiaTriad = mafia,
            CanBecomeKillingRole = killing,
            NewRoleRevealedToTown = announce
        });
        match.Perks[roleNames[2]].Defense = AttackStrength.None;
        match.Start();

        var amnesiac = match.AllPlayers[0];
        var killer = match.AllPlayers[1];
        var victim = match.AllPlayers[2];

        match.Skip<NightPhase>();

        if (attack) killer.Target(victim);
        if (remember) amnesiac.Target(victim);

        match.Skip<DeathsPhase>();

        Assert.That(amnesiac.Alive, Is.True);

        var dead = attack && victim.Perks.CurrentDefense == 0;
        Assert.That(victim.Alive, Is.EqualTo(!dead));
        Deaths(match, dead ? 1 : 0);

        match.Skip<NightPhase>();

        if (remember) amnesiac.Target(victim);
        var personalNotifications = new List<Text>();
        var personalMessage =
            Notification.Chat(amnesiac.Role, RememberKey.RememberPersonal, victim.Role).Localize();
        amnesiac.Chat += (s, e) =>
        {
            if (Equals(e, personalMessage)) personalNotifications.Add(e);
        };

        var announcements = new List<Text>();
        var publicAnnouncement = Notification.Popup(amnesiac.Role, RememberKey.RememberAnnouncement, victim.Role)
            .Localize();
        foreach (var player in match.AllPlayers)
            player.Popup += (s, e) =>
            {
                if (Equals(e, publicAnnouncement)) announcements.Add(e);
            };

        match.Skip<DeathsPhase>();

        var successful = Successful(victim, remember, town, mafia, killing);
        var isAmnesiac = !successful || victim.Role.Id == "Amnesiac"; // TODO: Based on actions
        Assert.That(amnesiac.Role.Id, isAmnesiac ? Is.EqualTo("Amnesiac") : Is.Not.EqualTo("Amnesiac"));

        if (isAmnesiac)
        {
            Assert.That(amnesiac.Role.Id, Is.EqualTo("Amnesiac"));
            Assert.That(amnesiac.Role.Name.ToString(), Is.EqualTo(roleNames[0]));
        }
        else
        {
            Assert.That(amnesiac.Abilities.All.Any(ability => ability is Remember), Is.False);
            Assert.That(amnesiac.Role.Name, Is.EqualTo(victim.Role.Name));
            Assert.That(amnesiac.Role.Summary, Is.EqualTo(victim.Role.Summary));
            Assert.That(amnesiac.Role.Team, Is.EqualTo(victim.Role.Team));
        }

        Assert.That(personalNotifications.Count, Is.EqualTo(successful ? 1 : 0));
        if (successful && announce)
            Assert.That(personalNotifications[0], Is.EqualTo(personalMessage));

        Assert.That(announcements.Count, Is.EqualTo(successful && announce ? roleNames.Length : 0));
        Assert.That(announcements, Is.All.EqualTo(publicAnnouncement));
    }
}

public class RememberCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var roleNames = "Amnesiac,Serial Killer,{0},Citizen,Citizen,Citizen";
        foreach (var role in RoleRegistry.Default.Entries())
        {
            if (!role.Natural) continue;

            var booleans = 6;
            var count = Math.Pow(2, booleans);

            for (var i = 0; i < count; i++)
            {
                IList<dynamic> args = Convert.ToString(i, 2)
                    .PadLeft(booleans, '0')
                    .Select(x => x == '1').Cast<dynamic>().ToList();

                args.Insert(0, string.Format(roleNames, role.Name));

                yield return args.ToArray();
            }
        }
    }
}