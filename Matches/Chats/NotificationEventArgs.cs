using System;

namespace Mafia.NET.Matches.Chats
{
    public class NotificationEventArgs : EventArgs, INotification
    {
        public NotificationType Type { get; }
        public string Text { get; }

        public NotificationEventArgs(NotificationType type, string text)
        {
            Type = type;
            Text = text;
        }

        public static NotificationEventArgs Chat(string text) => new NotificationEventArgs(NotificationType.CHAT, text);

        public static NotificationEventArgs Popup(string text) => new NotificationEventArgs(NotificationType.POPUP, text);
    }
}
