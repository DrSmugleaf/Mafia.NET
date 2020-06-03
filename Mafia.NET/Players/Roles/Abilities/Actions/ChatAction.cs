using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum ChatKey
    {
        Nickname,
        MayTalk
    }

    public class ChatAction<T> : AbilityAction where T : IChat, new()
    {
        public ChatAction(
            IAbility user,
            string id,
            bool muted = false,
            bool deaf = false,
            int priority = -1) :
            base(user, priority)
        {
            Id = id;
            Muted = muted;
            Deaf = deaf;
        }

        public string Id { get; set; }
        public bool Muted { get; set; }
        public bool Deaf { get; set; }

        public override bool Use()
        {
            var chat = Match.Chat.Open<T>(Id);
            var participant = chat.Get(User);
            participant.Nickname = new Key(Ability, ChatKey.Nickname);
            participant.Muted = Muted;
            participant.Deaf = Deaf;

            var notification = Notification.Chat(Ability, ChatKey.MayTalk);
            User.OnNotification(notification);

            return true;
        }
    }
}