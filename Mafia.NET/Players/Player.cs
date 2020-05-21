using System;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players
{
    public interface IPlayer : IColorizable
    {
        IPlayerController Controller { get; set; }
        IMatch Match { get; }
        int Number { get; }
        Guid Id { get; }
        string Name { get; set; }
        IRole Role { get; set; }
        bool Alive { get; set; }
        Note LastWill { get; }
        Note DeathNote { get; }
        bool Blackmailed { get; set; }
        Crimes Crimes { get; }
        CultureInfo Culture { get; }
        event EventHandler<Text> Chat;
        event EventHandler<Text> Popup;

        void OnNotification(Entry entry);
        void OnNotification(EntryBundle bundle);
    }

    public class Player : IPlayer
    {
        public Player(ILobbyController controller, IMatch match, int number, string name, IRole role,
            CultureInfo culture = null)
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
            Culture = culture ?? new CultureInfo("en-US");
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
        public CultureInfo Culture { get; }
        public event EventHandler<Text> Chat;
        public event EventHandler<Text> Popup;

        public void OnNotification(Entry entry)
        {
            var text = Localizer.Default.Get(entry, Culture);

            switch (text.Location)
            {
                case NotificationLocation.Chat:
                    Chat?.Invoke(this, text);
                    break;
                case NotificationLocation.Popup:
                    Popup?.Invoke(this, text);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void OnNotification(EntryBundle bundle)
        {
            foreach (var entry in bundle.Entries)
                OnNotification(entry);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}