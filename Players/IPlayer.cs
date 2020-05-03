using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Votes;
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
        event EventHandler<AccuseEventArgs> Accuse;
        event EventHandler<UnaccuseEventArgs> Unaccuse;
        event EventHandler<AccuseChangeEventArgs> AccuseChange;

        void OnAccuse(AccuseEventArgs e);
        void OnUnaccuse(UnaccuseEventArgs e);
        void OnAccuseChange(AccuseChangeEventArgs e);
        void OnNotification(NotificationEventArgs e);
    }
}
