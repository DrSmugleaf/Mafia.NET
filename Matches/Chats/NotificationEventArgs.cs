using System;
using System.Linq;

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

        public NotificationEventArgs(NotificationType type, params NotificationEventArgs[] notifications)
        {
            Type = type;
            Text = string.Join(Environment.NewLine, notifications.Select(notification => notification.Text));
        }

        public static NotificationEventArgs Chat(string text) => new NotificationEventArgs(NotificationType.CHAT, text);

        public static NotificationEventArgs Popup(string text) => new NotificationEventArgs(NotificationType.POPUP, text);
    }
}
