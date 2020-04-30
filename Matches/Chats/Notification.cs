namespace Mafia.NET.Matches.Chats
{
    public class Notification : INotification
    {
        public NotificationType Type { get; }
        public string Text { get; }

        public Notification(NotificationType type, string text)
        {
            Type = type;
            Text = text;
        }

        public static Notification Chat(string text) => new Notification(NotificationType.CHAT, text);

        public static Notification Popup(string text) => new Notification(NotificationType.POPUP, text);
    }
}
