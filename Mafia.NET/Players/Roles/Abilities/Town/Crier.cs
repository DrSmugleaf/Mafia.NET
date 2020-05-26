using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CrierKey
    {
        MayTalk,
        Nickname
    }

    [RegisterAbility("Crier", typeof(CrierSetup))]
    public class Crier : TownAbility<CrierSetup>
    {
        public override void Chat()
        {
            var chat = Match.Chat.Open<CrierChat>(CrierChat.Name);
            var participant = chat.Get(User);
            participant.Nickname = CrierKey.Nickname;
            participant.Muted = false;

            var notification = Notification.Chat(CrierKey.MayTalk);
            User.OnNotification(notification);
        }
    }

    public class CrierSetup : ITownSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = false;
    }

    public interface ICrierChatter : IAbility
    {
    }

    public class CrierChat : Chat
    {
        public static readonly string Name = "Crier";

        public CrierChat() : base(Name)
        {
        }

        public override void Initialize(IMatch match)
        {
            if (Initialized) return;

            foreach (var player in match.AllPlayers)
                if (player.Role.Ability is ICrierChatter)
                    Mute(player);
                else Mute(player, false);

            Initialized = true;
        }
    }
}