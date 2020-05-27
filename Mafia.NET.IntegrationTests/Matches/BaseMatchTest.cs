using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Matches
{
    public abstract class BaseMatchTest
    {
        protected void Deaths(IMatch match, int deaths)
        {
            var roles = match.Setup.Roles.Selectors.Count;

            Assert.That(match.AllPlayers, Has.None.Null);
            Assert.That(match.AllPlayers.Count, Is.EqualTo(roles));
            Assert.That(match.LivingPlayers.Count, Is.EqualTo(roles - deaths));
            Assert.That(match.Graveyard.AllDeaths().Count, Is.EqualTo(deaths));
        }

        protected void Messages(
            IList<MessageOut> messages,
            int amount,
            IPlayer sender,
            string text,
            [CanBeNull] Key nickname,
            params IPlayer[] listeners)
        {
            Assert.That(messages.Count, Is.EqualTo(amount));

            foreach (var message in messages)
            {
                Assert.That(message.Listeners.Count, Is.EqualTo(listeners.Length));
                foreach (var listener in listeners)
                    Assert.That(message.Listeners, Does.Contain(listener));
                
                Assert.That(message.Sender.Owner, Is.EqualTo(sender));
                Assert.That(message.Text, Is.EqualTo(text));

                Assert.That(message.Sender.Nickname, Is.Not.Null);
                Assert.That(message.Sender.Nickname, Is.EqualTo(nickname));

                Assert.That(message.DisplayText(sender).String, Does.Not.Contain(sender.Name));

                if (nickname != null)
                    Assert.That(message.DisplayText(sender).String, Does.StartWith(nickname.ToString()));
            }
        }
    }
}