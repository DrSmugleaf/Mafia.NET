using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles;
using System.Drawing;

namespace Mafia.NET.Players
{
    public interface IPlayer
    {
        int Id { get; }
        string Name { get; }
        IRole Role { get; set; }
        Color Tint { get; }

        public void OnNotification(NotificationEventArgs e);
    }
}
