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
        string Name { get; set; }
        IRole Role { get; set; }
        Color Tint { get; }
        bool Alive { get; set; }
#nullable enable
        IPlayer? Accuses { get; set; }
#nullable disable
        bool Anonymous { get; set; }
        string LastWill { get; set; }
        string DeathNote { get; set; }
        event EventHandler<AccuseEventArgs> Accuse;
        event EventHandler<UnaccuseEventArgs> Unaccuse;
        event EventHandler<AccuseChangeEventArgs> AccuseChange;
        event EventHandler<Notification> Notification;
        event EventHandler<Message> Message;

        void OnAccuse(AccuseEventArgs e);
        void OnUnaccuse(UnaccuseEventArgs e);
        void OnAccuseChange(AccuseChangeEventArgs e);
        void OnNotification(Notification e);
        void OnMessage(Message e);
    }
}
