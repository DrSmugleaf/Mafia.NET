using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Crier", typeof(CrierSetup))]
    public class Crier : TownAbility<CrierSetup>, INightChatter
    {
        public static readonly string NightChatName = "Crier Chat";

        public void Chat()
        {
            Match.Chat.Open(new CrierChat(Match));

            var notification = Notification.Chat("You may now talk to the town anonymously.");
            User.OnNotification(notification);
        }
    }

    public class CrierSetup : ITownSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = false;
    }

    public class CrierChat : Chat
    {
        public CrierChat(IMatch match) : base(Crier.NightChatName)
        {
            foreach (var player in match.AllPlayers)
            {
                var participant = new ChatParticipant(player, true);
                if (player.Role.Ability is Crier)
                {
                    participant.Name = "Crier";
                    participant.Muted = false;
                }

                Participants[player] = participant;
            }
        }
    }
}