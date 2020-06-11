using System;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities.Managers;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players
{
    public interface IPlayer : IColorizable
    {
        bool Alive { get; set; }
        IPlayerController Controller { get; set; }
        IMatch Match { get; }
        int Number { get; }
        Guid Id { get; }
        Text Name { get; set; }
        IRole Role { get; }
        Note LastWill { get; }
        Note DeathNote { get; }
        Crimes Crimes { get; }
        CultureInfo Culture { get; }
        TargetManager Targets { get; }
        AbilityManager Abilities { get; }
        PerkManager Perks { get; }
        event EventHandler<Text> Chat;
        event EventHandler<Text> Popup;

        void OnNotification(Notification notification);
        void OnNotification(EntryBundle bundle);
        void ChangeRole(IRole role);
        void Target(IPlayer target);
        void OnDayStart();
        void OnDayEnd();
        void OnNightStart();
        void BeforeNightEnd();
        void OnNightEnd();
    }

    public class Player : IPlayer
    {
        public Player(
            ILobbyController controller,
            IMatch match,
            int number,
            string name,
            IRole role,
            CultureInfo culture = null)
        {
            Alive = true;
            Match = match;
            Number = number;
            Id = controller.Id;
            Color = PlayerColorsExtensions.From(number).Color();
            Name = new Text(new[] {new Content(name, Color)}, Color.Empty);
            Role = role;
            LastWill = new Note(Match, this);
            DeathNote = new Note(Match, this);
            Crimes = new Crimes(this);
            Controller = controller.Player(this);
            Culture = culture ?? new CultureInfo("en-US");
            Targets = new TargetManager(this);
        }

        public bool Alive { get; set; }
        public IPlayerController Controller { get; set; }
        public IMatch Match { get; }
        public int Number { get; }
        public Guid Id { get; }
        public Color Color { get; }
        public Text Name { get; set; }
        public IRole Role { get; protected set; }
        public Note LastWill { get; }
        public Note DeathNote { get; }
        public Crimes Crimes { get; }
        public TargetManager Targets { get; }
        public CultureInfo Culture { get; }
        public AbilityManager Abilities => Role.Abilities;
        public PerkManager Perks => Role.Perks;
        public event EventHandler<Text> Chat;
        public event EventHandler<Text> Popup;

        public void OnNotification(Notification notification)
        {
            var text = Localizer.Default.Get(notification.Key, Culture, notification.Args);

            switch (notification.Location)
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

        public void ChangeRole(IRole role)
        {
            Role = role;
            Role.Initialize(this);
        }

        public void Target(IPlayer target)
        {
            Targets.Set(target);
        }

        public void OnDayStart()
        {
            Crimes.OnDayStart();
            Perks.OnDayStart();
        }

        public void OnDayEnd()
        {
            Targets.Reset(Time.Night);
        }

        public void OnNightStart()
        {
        }

        public void BeforeNightEnd()
        {
            Perks.BeforeNightEnd();
        }

        public void OnNightEnd()
        {
            Targets.Reset(Time.Day);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}