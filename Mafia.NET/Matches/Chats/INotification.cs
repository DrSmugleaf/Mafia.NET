namespace Mafia.NET.Matches.Chats
{
    public interface INotification
    {
        NotificationType Type { get; }
        string Text { get; }
    }
}