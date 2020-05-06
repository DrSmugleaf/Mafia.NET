using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Chats
{
    public class Notification : EventArgs, INotification
    {
        public NotificationType Type { get; }
        public string Text { get; }

        public Notification(NotificationType type, string text)
        {
            Type = type;
            Text = text;
        }

        public Notification(NotificationType type, IEnumerable<Notification> notifications)
        {
            Type = type;
            Text = string.Join(Environment.NewLine, notifications.Select(notification => notification.Text));
        }

        public Notification(NotificationType type, params Notification[] notifications) : this(type, notifications.AsEnumerable())
        {
        }

        public static Notification Chat(string text) => new Notification(NotificationType.CHAT, text);

        public static Notification Popup(string text) => new Notification(NotificationType.POPUP, text);
    }
}
