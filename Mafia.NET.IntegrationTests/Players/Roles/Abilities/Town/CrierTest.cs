using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Town;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Town
{
    [TestFixture]
    [TestOf(typeof(Crier))]
    public class CrierTest : BaseMatchTest
    {
        [TestCase("Crier,Citizen,Mafioso")]
        public void NightChat(string namesString)
        {
            var roleNames = namesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var crier = match.AllPlayers[0];

            match.Skip<NightPhase>();

            var text = "Hey there folks, DJ Crier here";
            var messages = match.Chat.Send(crier, text);

            Assert.That(messages.Count, Is.EqualTo(1));

            foreach (var message in messages)
            {
                Assert.That(message.Listeners.Count, Is.EqualTo(roleNames.Length));
                Assert.That(message.Sender.Owner, Is.EqualTo(crier));
                Assert.That(message.Text, Is.EqualTo(text));
                
                var nickname = new Key(CrierKey.Nickname);
                Assert.That(message.Sender.Nickname, Is.Not.Null);
                Assert.That(message.Sender.Nickname, Is.EqualTo(nickname));
                
                Assert.That(message.DisplayText(crier).String, Does.Not.Contain(crier.Name));
                Assert.That(message.DisplayText(crier).String, Does.StartWith(nickname.ToString()));
            }
        }
    }
}