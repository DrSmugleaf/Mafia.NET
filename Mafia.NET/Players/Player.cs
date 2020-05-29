using System;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Controllers;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;

namespace Mafia.NET.Players
{
    public interface IPlayer : IColorizable
    {
        IPlayerController Controller { get; set; }
        IMatch Match { get; }
        int Number { get; }
        Guid Id { get; }
        Text Name { get; set; }
        IRole Role { get; set; }
        bool Alive { get; set; }
        Note LastWill { get; }
        Note DeathNote { get; }
        bool Blackmailed { get; set; }
        bool Doused { get; set; }
        Crimes Crimes { get; }
        CultureInfo Culture { get; }
        TargetManager TargetManager { get; }
        event EventHandler<Text> Chat;
        event EventHandler<Text> Popup;

        void OnNotification(Notification notification);
        void OnNotification(EntryBundle bundle);
        void ChangeRole(IRole role);
        void ChangeRole(IAbility ability);
        void Target(IPlayer target);
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
            Match = match;
            Number = number;
            Id = controller.Id;
            Color = PlayerColorsExtensions.From(number).Color();
            Name = new Text(new[] {new Content(name, Color)}, Color.Empty);
            Role = role;
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
        public Color Color { get; }
        public Text Name { get; set; }
        public IRole Role { get; set; }
        public bool Alive { get; set; }
        public Note LastWill { get; }
        public Note DeathNote { get; }
        public bool Blackmailed { get; set; }
        public bool Doused { get; set; }
        public Crimes Crimes { get; }
        public CultureInfo Culture { get; }
        public TargetManager TargetManager => Role.Ability.TargetManager;
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
            role.Ability.User = this;
        }

        public void ChangeRole(IAbility ability)
        {
            var roleEntry = Match.RoleSetup.Roles.Names[ability.Name];
            var role = new Role(roleEntry, ability);
            ChangeRole(role);
        }

        public void Target(IPlayer target)
        {
            TargetManager.Set(target);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}