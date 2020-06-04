using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia
{
    [TestFixture]
    [TestOf(typeof(Janitor))]
    public class JanitorTest : BaseMatchTest
    {
        [TestCaseSource(typeof(SanitizeCases))]
        public void Sanitize(string rolesString, bool sanitize, int uses)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Set(new JanitorSetup
            {
                Uses = uses
            });
            match.Start();

            var janitor = match.AllPlayers[0];
            var mafioso = match.AllPlayers[1];
            var citizen = match.AllPlayers[2];

            match.Skip<NightPhase>();
            
            if (sanitize) janitor.Target(citizen);
            mafioso.Target(citizen);
            
            var lw = "LW";
            citizen.LastWill.Text = lw;
            
            var chat = new List<string>();
            var popups = new List<string>();
            foreach (var player in match.AllPlayers)
            {
                player.Chat += (s, e) => chat.Add(e.ToString());
                player.Popup += (s, e) => popups.Add(e.ToString());
            }

            var janitorChat = new List<string>();
            janitor.Chat += (s, e) => janitorChat.Add(e.ToString());

            match.Skip<DeathsPhase>();

            var role = citizen.Role.Name.Localize().ToString();
            var roleUnknown = new Key(DayKey.DeathRoleUnknown).ToString();
            var lwUnknown = new Key(DayKey.LastWillUnknown).ToString();
            var janitorLw = Notification.Chat(janitor.Ability, SanitizeKey.LastWillReveal, lw).ToString();
            if (sanitize)
            {
                Assert.That(popups, Has.None.Contain(role));
                Assert.That(popups, Has.Some.EqualTo(roleUnknown));
                Assert.That(chat, Has.None.EqualTo(lw));
                Assert.That(chat, Has.Some.EqualTo(lwUnknown));
                Assert.That(janitorChat, Has.One.EqualTo(janitorLw));
                Assert.That(janitorChat, Has.None.EqualTo(role));
            }
            else
            {
                Assert.That(popups, Has.Some.Contain(role));
                Assert.That(popups, Has.None.EqualTo(roleUnknown));
                Assert.That(chat, Has.Some.EqualTo(lw));
                Assert.That(chat, Has.None.EqualTo(lwUnknown));
                
                Assert.That(janitorChat, Has.None.EqualTo(janitorLw));
                Assert.That(janitorChat, Has.None.EqualTo(role));
            }
        }
    }
    
    public class SanitizeCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var janitors = new[] {"Janitor"};

            foreach (var janitor in janitors)
            {
                var roleNames = $"{janitor},Mafioso,Citizen,Citizen,Citizen,Citizen";

                foreach (var sanitize in new[] {true, false})
                    yield return new object[] {roleNames, sanitize, 2};
            }
        }
    }
}