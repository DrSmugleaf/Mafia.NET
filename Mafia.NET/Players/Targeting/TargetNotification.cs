using System;
using JetBrains.Annotations;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Targeting
{
    public class TargetNotification
    {
        public static readonly TargetNotification Empty = new TargetNotification();
        public static readonly Func<IPlayer, Notification> Default = player => Notification.Empty;

        public static readonly Func<IPlayer, IPlayer, Notification>
            DefaultChange = (old, current) => Notification.Empty;

        public TargetNotification(
            Func<IPlayer, Notification> userAdd = null,
            Func<IPlayer, Notification> userRemove = null,
            Func<IPlayer, IPlayer, Notification> userChange = null,
            Func<IPlayer, Notification> targetAdd = null,
            Func<IPlayer, Notification> targetRemove = null,
            Func<IPlayer, IPlayer, Notification> targetChange = null)
        {
            UserAddMessage = userAdd;
            UserRemoveMessage = userRemove;
            UserChangeMessage = userChange;
            TargetAddMessage = targetAdd;
            TargetRemoveMessage = targetRemove;
            TargetChangeMessage = targetChange;
        }

        [CanBeNull] public Func<IPlayer, Notification> UserAddMessage { get; set; }
        [CanBeNull] public Func<IPlayer, Notification> UserRemoveMessage { get; set; }
        [CanBeNull] public Func<IPlayer, IPlayer, Notification> UserChangeMessage { get; set; }
        [CanBeNull] public Func<IPlayer, Notification> TargetAddMessage { get; set; }
        [CanBeNull] public Func<IPlayer, Notification> TargetRemoveMessage { get; set; }
        [CanBeNull] public Func<IPlayer, IPlayer, Notification> TargetChangeMessage { get; set; }

        public static TargetNotification Enum<T>(IAbility ability) where T : Enum
        {
            var type = typeof(T);
            var names = System.Enum.GetValues(type);

            Func<IPlayer, Notification> userAdd = null;
            Func<IPlayer, Notification> userRemove = null;
            Func<IPlayer, IPlayer, Notification> userChange = null;
            Func<IPlayer, Notification> targetAdd = null;
            Func<IPlayer, Notification> targetRemove = null;
            Func<IPlayer, IPlayer, Notification> targetChange = null;
            var role = ability.Role;
            foreach (T key in names)
                switch (key.ToString())
                {
                    case "UserAddMessage":
                        userAdd = target => Notification.Chat(role, key, target);
                        break;
                    case "UserRemoveMessage":
                        userRemove = target => Notification.Chat(role, key, target);
                        break;
                    case "UserChangeMessage":
                        userChange = (old, current) => Notification.Chat(role, key, old, current);
                        break;
                    case "TargetAddMessage":
                        targetAdd = target => Notification.Chat(role, key, target);
                        break;
                    case "TargetRemoveMessage":
                        targetRemove = target => Notification.Chat(role, key, target);
                        break;
                    case "TargetChangeMessage":
                        targetChange = (old, current) => Notification.Chat(role, key, old, current);
                        break;
                }

            return new TargetNotification(userAdd, userRemove, userChange, targetAdd, targetRemove, targetChange);
        }

        public Notification UserAdd(IPlayer target)
        {
            return (UserAddMessage ?? Default)(target);
        }

        public Notification UserRemove(IPlayer target)
        {
            return (UserRemoveMessage ?? Default)(target);
        }

        public Notification UserChange(IPlayer old, IPlayer current)
        {
            return (UserChangeMessage ?? DefaultChange)(old, current);
        }

        public Notification TargetAdd(IPlayer target)
        {
            return (TargetAddMessage ?? Default)(target);
        }

        public Notification TargetRemove(IPlayer target)
        {
            return (TargetRemoveMessage ?? Default)(target);
        }

        public Notification TargetChange(IPlayer old, IPlayer current)
        {
            return (TargetChangeMessage ?? DefaultChange)(old, current);
        }
    }
}