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
using Mafia.NET.Players.Roles.Abilities.Neutral;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(Amnesiac))]
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
                .Any(category => category.Id.EndsWith("Killing"));

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
            string namesString,
            bool attack,
            bool remember,
            bool announce,
            bool town,
            bool mafia,
            bool killing)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new AmnesiacSetup
            {
                CanBecomeTown = town,
                CanBecomeMafiaTriad = mafia,
                CanBecomeKillingRole = killing,
                NewRoleRevealedToTown = announce
            });
            match.Start();

            var amnesiac = match.AllPlayers[0];
            var killer = match.AllPlayers[1];
            var victim = match.AllPlayers[2];

            match.Skip<NightPhase>();

            if (attack) killer.TargetManager.Set(victim);
            if (remember) amnesiac.TargetManager.Set(victim);

            match.Skip<DeathsPhase>();

            Assert.That(amnesiac.Alive, Is.True);

            var dead = victim.Role.Ability.AbilitySetup is INightImmune nImmune
                ? !nImmune.NightImmune && attack
                : attack;
            Assert.That(victim.Alive, Is.EqualTo(!dead));
            Deaths(match, dead ? 1 : 0);

            match.Skip<NightPhase>();

            if (remember) amnesiac.TargetManager.Set(victim);
            var personalNotifications = new List<Text>();
            var personalMessage = Notification.Chat(AmnesiacKey.RememberPersonal, victim.Role).Localize();
            amnesiac.Chat += (s, e) =>
            {
                if (Equals(e, personalMessage)) personalNotifications.Add(e);
            };

            var announcements = new List<Text>();
            var publicAnnouncement = Notification.Popup(AmnesiacKey.RememberAnnouncement, victim.Role).Localize();
            foreach (var player in match.AllPlayers)
                player.Popup += (s, e) =>
                {
                    if (Equals(e, publicAnnouncement)) announcements.Add(e);
                };

            match.Skip<DeathsPhase>();

            var successful = Successful(victim, remember, town, mafia, killing);
            var isAmnesiac = successful && !(victim.Role.Ability is Amnesiac);
            Assert.That(amnesiac.Role.Ability, isAmnesiac ? Is.Not.TypeOf<Amnesiac>() : Is.TypeOf<Amnesiac>());

            if (successful)
            {
                Assert.That(amnesiac.Role.Ability.User, Is.EqualTo(amnesiac));
                Assert.That(amnesiac.Role.Name, Is.EqualTo(victim.Role.Name));
                Assert.That(amnesiac.Role.Summary, Is.EqualTo(victim.Role.Summary));
                Assert.That(amnesiac.Role.Team, Is.EqualTo(victim.Role.Team));
            }
            else
            {
                Assert.That(amnesiac.Role.Ability.User, Is.EqualTo(amnesiac));
                Assert.That(amnesiac.Role.Name.ToString(), Is.EqualTo(roleNames[0]));
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
            // TODO: Change to RoleRegistry once all the abilities are done
            foreach (var role in AbilityRegistry.Default.Names.Keys)
            {
                if (!RoleRegistry.Default.Names[role].Natural) continue;

                var booleans = 6;
                var count = Math.Pow(2, booleans);

                for (var i = 0; i < count; i++)
                {
                    IList<dynamic> args = Convert.ToString(i, 2)
                        .PadLeft(booleans, '0')
                        .Select(x => x == '1').Cast<dynamic>().ToList();

                    args.Insert(0, string.Format(roleNames, role));

                    yield return args.ToArray();
                }
            }
        }
    }
}