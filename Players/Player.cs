using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles;
using System;
using System.Drawing;

namespace Mafia.NET.Players
{
    public class Player : IPlayer
    {
        public int Id { get; }
        public string Name { get; }
        public IRole Role { get; set; }
        public Color Tint { get; }
        public event EventHandler<NotificationEventArgs> Notification;

        public Player(int id, string name, IRole role)
        {
            Id = id;
            Name = name;
            Role = role;
            Tint = IdToColor(id);
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
                _ => PlayerColors.GRAY
            };

            return Color.FromArgb((int)color);
        }

        public virtual void OnNotification(NotificationEventArgs e)
        {
            Notification?.Invoke(this, e);
        }
    }
}
