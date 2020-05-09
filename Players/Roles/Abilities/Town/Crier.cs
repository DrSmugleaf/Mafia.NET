using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Crier", typeof(CrierSetup))]
    public class Crier : TownAbility<CrierSetup>, INightChatter
    {
        public static readonly string NightChatName = "Crier Chat";

        public void Chat()
        {
            var chat = Match.Chat.Open(Match.AllPlayers.Values, true, NightChatName);
            var crier = chat.Participants[User];
            crier.Name = "Crier";
            crier.Muted = false;

            var notification = Notification.Chat("You may now talk to the town anonymously.");
            User.OnNotification(notification);
        }
    }

    public class CrierSetup : ITownSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = false;
    }
}
