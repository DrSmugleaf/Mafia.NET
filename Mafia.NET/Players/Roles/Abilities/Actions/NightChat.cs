using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum ChatKey
    {
        Nickname,
        MayTalk
    }

    public class NightChat<T> : NightStartAbility where T : IChat, new()
    {
        public virtual string ChatId { get; set; }
        public bool Muted { get; set; }
        public bool Deaf { get; set; }

        public override bool Use()
        {
            // TODO: Default to team as id? (Mafia, Triad, Cultist, Mason)
            var chat = Match.Chat.Open<T>(ChatId);
            var participant = chat.Get(User);
            participant.Nickname = new Key(Role, ChatKey.Nickname);
            participant.Muted = Muted;
            participant.Deaf = Deaf;

            var notification = Notification.Chat(Role, ChatKey.MayTalk);
            User.OnNotification(notification);

            return true;
        }
    }

    public class NightChat : NightChat<Chat>
    {
    }
}