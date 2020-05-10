using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Chats
{
    public class Notification : EventArgs, INotification
    {
        public Notification(NotificationType type, string text)
        {
            Type = type;
            Text = text.Trim();
        }

        public Notification(NotificationType type, IEnumerable<Notification> notifications)
        {
            Type = type;
            Text = string.Join(Environment.NewLine, notifications.Select(notification => notification.Text)).Trim();
        }

        public Notification(NotificationType type, params Notification[] notifications) : this(type,
            notifications.AsEnumerable())
        {
        }

        public NotificationType Type { get; }
        public string Text { get; }

        public static Notification Chat(string text)
        {
            return new Notification(NotificationType.CHAT, text);
        }

        public static Notification Popup(string text)
        {
            return new Notification(NotificationType.POPUP, text);
        }
    }
}