using System.Linq;
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
            var nickname = new Key(CrierKey.Nickname);

            Messages(messages, 1, crier, text, nickname, match.AllPlayers.ToArray());
        }
    }
}