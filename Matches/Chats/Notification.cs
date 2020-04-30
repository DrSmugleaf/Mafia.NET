namespace Mafia.NET.Matches.Chats
{
    public class Notification : INotification
    {
        public NotificationType Type { get; }

        public Notification(NotificationType type)
        {
            Type = type;
        }
    }
}
