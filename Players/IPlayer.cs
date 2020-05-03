using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players.Roles;
using System;
using System.Drawing;

namespace Mafia.NET.Players
{
    public interface IPlayer
    {
        int Id { get; }
        string Name { get; }
        IRole Role { get; set; }
        Color Tint { get; }
        bool Alive { get; set; }
#nullable enable
        IPlayer? Accuses { get; set; }
#nullable disable
        bool Anonymous { get; set; }
        event EventHandler<NotificationEventArgs> Notification;
        public event EventHandler<AccuseEventArgs> Accuse;
        public event EventHandler<UnaccuseEventArgs> Unaccuse;

        public void OnNotification(NotificationEventArgs e);
    }
}
