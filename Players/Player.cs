using Mafia.NET.Matches;
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
        IMatch Match { get; }
        int Id { get; }
        string Name { get; set; }
        IRole Role { get; set; }
        Color Tint { get; }
        bool Alive { get; set; }
#nullable enable
        IPlayer? Accuses { get; set; }
#nullable disable
        bool Anonymous { get; set; }
        Note LastWill { get; }
        Note DeathNote { get; }
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

    public class Player : IPlayer
    {
        public IMatch Match { get; }
        public int Id { get; }
        public string Name { get; set; }
        public IRole Role { get; set; }
        public Color Tint { get; }
        public bool Alive { get; set; }
#nullable enable
        private IPlayer? _accuses { get; set; }
        public IPlayer? Accuses
        {
            get => _accuses;
            set
            {
                var old = _accuses;
                _accuses = value;

                if (Accuses == null && old != null)
                {
                    var ev = new UnaccuseEventArgs(this, old);
                    OnUnaccuse(ev);
                }
                else if (Accuses != null && old == null)
                {
                    var ev = new AccuseEventArgs(this, Accuses);
                    OnAccuse(ev);
                }
                else if (Accuses != null && old != null)
                {
                    var ev = new AccuseChangeEventArgs(this, old, Accuses);
                    OnAccuseChange(ev);
                }
            }
        }
#nullable disable
        public bool Anonymous { get; set; }
        public Note LastWill { get; }
        public Note DeathNote { get; }
        public event EventHandler<AccuseEventArgs> Accuse;
        public event EventHandler<UnaccuseEventArgs> Unaccuse;
        public event EventHandler<AccuseChangeEventArgs> AccuseChange;
        public event EventHandler<Notification> Notification;
        public event EventHandler<Message> Message;

        public Player(IMatch match, int id, string name, IRole role, bool anonymous)
        {
            Match = match;
            Id = id;
            Name = name;
            Role = role;
            Tint = IdToColor(id);
            Anonymous = anonymous;
            LastWill = new Note(Match, this);
            DeathNote = new Note(Match, this);
        }

        public static Color IdToColor(int id)
        {
            var color = id switch
            {
                1 => PlayerColors.RED,
                2 => PlayerColors.BLUE,
                3 => PlayerColors.TEAL,
                4 => PlayerColors.PURPLE,
                5 => PlayerColors.YELLOW,
                6 => PlayerColors.ORANGE,
                7 => PlayerColors.GREEN,
                8 => PlayerColors.LIGHT_PINK,
                9 => PlayerColors.VIOLET,
                10 => PlayerColors.GREY,
                11 => PlayerColors.DARK_GREEN,
                12 => PlayerColors.BROWN,
                13 => PlayerColors.LIGHT_GREEN,
                14 => PlayerColors.BLACK,
                15 => PlayerColors.PINK,
                _ => PlayerColors.DEFAULT
            };

            return Color.FromArgb((int)color);
        }

        public virtual void OnAccuse(AccuseEventArgs e) => Accuse?.Invoke(this, e);

        public virtual void OnUnaccuse(UnaccuseEventArgs e) => Unaccuse?.Invoke(this, e);

        public virtual void OnAccuseChange(AccuseChangeEventArgs e) => AccuseChange?.Invoke(this, e);

        public virtual void OnNotification(Notification e) => Notification?.Invoke(this, e);

        public virtual void OnMessage(Message e) => Message?.Invoke(this, e);
    }
}
