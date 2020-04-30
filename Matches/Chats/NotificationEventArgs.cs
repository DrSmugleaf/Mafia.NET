using System;

namespace Mafia.NET.Matches.Chats
{
    public class NotificationEventArgs : EventArgs
    {
        public Notification Notification { get; set; }

        public NotificationEventArgs(Notification notification)
        {
            Notification = notification;
        }
    }
}
