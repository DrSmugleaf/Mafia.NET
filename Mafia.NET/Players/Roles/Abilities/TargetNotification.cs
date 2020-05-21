using System;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetNotification
    {
        public static readonly TargetNotification Empty = new TargetNotification();
        public static readonly Func<IPlayer, Notification> Default = player => Notification.Empty;
        public static readonly Func<IPlayer, IPlayer, Notification> DefaultChange = (old, current) => Notification.Empty;

        public TargetNotification(
            Func<IPlayer, Notification> userAdd = null,
            Func<IPlayer, Notification> userRemove = null,
            Func<IPlayer, IPlayer, Notification> userChange = null,
            Func<IPlayer, Notification> targetAdd = null,
            Func<IPlayer, Notification> targetRemove = null,
            Func<IPlayer, IPlayer, Notification> targetChange = null)
        {
            UserAddMessage = userAdd ?? Default;
            UserRemoveMessage = userRemove ?? Default;
            UserChangeMessage = userChange ?? DefaultChange;
            TargetAddMessage = targetAdd ?? Default;
            TargetRemoveMessage = targetRemove ?? Default;
            TargetChangeMessage = targetChange ?? DefaultChange;
        }

        public Func<IPlayer, Notification> UserAddMessage { get; set; }
        public Func<IPlayer, Notification> UserRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification> UserChangeMessage { get; set; }
        public Func<IPlayer, Notification> TargetAddMessage { get; set; }
        public Func<IPlayer, Notification> TargetRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification> TargetChangeMessage { get; set; }

        public static TargetNotification Enum<T>() where T : Enum
        {
            var type = typeof(T);
            var names = System.Enum.GetValues(type);

            Func<IPlayer, Notification> userAdd = null;
            Func<IPlayer, Notification> userRemove = null;
            Func<IPlayer, IPlayer, Notification> userChange = null;
            Func<IPlayer, Notification> targetAdd = null;
            Func<IPlayer, Notification> targetRemove = null;
            Func<IPlayer, IPlayer, Notification> targetChange = null;
            foreach (T key in names)
            {
                switch (key.ToString())
                {
                    case "UserAddMessage":
                        userAdd = target => Notification.Chat(key, target);
                        break;
                    case "UserRemoveMessage":
                        userRemove = target => Notification.Chat(key, target);
                        break;
                    case "UserChangeMessage":
                        userChange = (old, current) => Notification.Chat(key, old, current);
                        break;
                    case "TargetAddMessage":
                        targetAdd = target => Notification.Chat(key, target);
                        break;
                    case "TargetRemoveMessage":
                        targetRemove = target => Notification.Chat(key, target);
                        break;
                    case "TargetChangeMessage":
                        targetChange = (old, current) => Notification.Chat(key, old, current);
                        break;
                }
            }

            return new TargetNotification(userAdd, userRemove, userChange, targetAdd, targetRemove, targetChange);
        }

        public Notification UserAdd(IPlayer target)
        {
            return UserAddMessage(target);
        }

        public Notification UserRemove(IPlayer target)
        {
            return UserRemoveMessage(target);
        }

        public Notification UserChange(IPlayer old, IPlayer current)
        {
            return UserChangeMessage(old, current);
        }

        public Notification TargetAdd(IPlayer target)
        {
            return TargetAddMessage(target);
        }

        public Notification TargetRemove(IPlayer target)
        {
            return TargetRemoveMessage(target);
        }

        public Notification TargetChange(IPlayer old, IPlayer current)
        {
            return TargetChangeMessage(old, current);
        }
    }
}