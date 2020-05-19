using System;
using System.Drawing;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players
{
    public interface IPlayer
    {
        IPlayerController Controller { get; set; }
        IMatch Match { get; }
        int Number { get; }
        Guid Id { get; }
        string Name { get; set; }
        IRole Role { get; set; }
        Color Color { get; }
        bool Alive { get; set; }
        Note LastWill { get; }
        Note DeathNote { get; }
        bool Blackmailed { get; set; }
        Crimes Crimes { get; }
        event EventHandler<Notification> Notification;

        void OnNotification(Notification e);
    }

    public class Player : IPlayer
    {
        public Player(ILobbyController controller, IMatch match, int number, string name, IRole role)
        {
            Match = match;
            Number = number;
            Id = controller.Id;
            Name = name;
            Role = role;
            Color = PlayerColorsExtensions.From(number).Color();
            Alive = true;
            LastWill = new Note(Match, this);
            DeathNote = new Note(Match, this);
            Blackmailed = false;
            Crimes = new Crimes(this);
            Controller = controller.Player(this);
        }

        public IPlayerController Controller { get; set; }
        public IMatch Match { get; }
        public int Number { get; }
        public Guid Id { get; }
        public string Name { get; set; }
        public IRole Role { get; set; }
        public Color Color { get; }
        public bool Alive { get; set; }
        public Note LastWill { get; }
        public Note DeathNote { get; }
        public bool Blackmailed { get; set; }
        public Crimes Crimes { get; }
        public event EventHandler<Notification> Notification;

        public virtual void OnNotification(Notification e)
        {
            if (e.Text.Length == 0) return;
            Notification?.Invoke(this, e);
        }
    }
}