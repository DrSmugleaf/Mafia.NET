using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CrierKey
    {
        MayTalk
    }

    [RegisterAbility("Crier", typeof(CrierSetup))]
    public class Crier : TownAbility<CrierSetup>
    {
        public static readonly string NightChatId = "Crier Chat";

        public override void Chat()
        {
            Match.Chat.Open(new CrierChat(Match));

            var notification = Notification.Chat(CrierKey.MayTalk);
            User.OnNotification(notification);
        }
    }

    public class CrierSetup : ITownSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = false;
    }

    public class CrierChat : Chat
    {
        public CrierChat(IMatch match) : base(Crier.NightChatId)
        {
            foreach (var player in match.AllPlayers)
            {
                var participant = new ChatParticipant(player, true);
                if (player.Role.Ability is Crier)
                {
                    participant.Name = "Crier"; // TODO: Localize
                    participant.Muted = false;
                }

                Participants[player] = participant;
            }
        }
    }
}