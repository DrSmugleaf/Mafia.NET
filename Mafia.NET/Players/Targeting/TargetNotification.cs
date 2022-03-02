using System;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Targeting
{
    public class TargetNotification
    {
        public static readonly TargetNotification Empty = new TargetNotification();

        public static readonly Func<IPlayer, Notification> DefaultUser = target => Notification.Empty;
        public static readonly Func<IPlayer, IPlayer?, Notification> DefaultOther = (user, target) => Notification.Empty;

        public static readonly Func<IPlayer, IPlayer, Notification>
            DefaultChangeUser = (old, current) => Notification.Empty;

        public static readonly Func<IPlayer, IPlayer, IPlayer, Notification>
            DefaultChangeOther = (user, old, current) => Notification.Empty;

        public TargetNotification(
            Func<IPlayer, Notification>? userAdd = null,
            Func<IPlayer, Notification>? userRemove = null,
            Func<IPlayer, IPlayer, Notification>? userChange = null,
            Func<IPlayer, IPlayer, Notification>? targetAdd = null,
            Func<IPlayer, IPlayer, Notification>? targetRemove = null,
            Func<IPlayer, IPlayer, IPlayer, Notification>? targetChange = null,
            Func<IPlayer, IPlayer?, Notification>? teamAdd = null,
            Func<IPlayer, IPlayer, Notification>? teamRemove = null,
            Func<IPlayer, IPlayer, IPlayer, Notification>? teamChange = null)
        {
            UserAddMessage = userAdd;
            UserRemoveMessage = userRemove;
            UserChangeMessage = userChange;
            TargetAddMessage = targetAdd;
            TargetRemoveMessage = targetRemove;
            TargetChangeMessage = targetChange;
            TeamAddMessage = teamAdd;
            TeamRemoveMessage = teamRemove;
            TeamChangeMessage = teamChange;
        }

        public Func<IPlayer, Notification>? UserAddMessage { get; set; }
        public Func<IPlayer, Notification>? UserRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification>? UserChangeMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification>? TargetAddMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification>? TargetRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, IPlayer, Notification>? TargetChangeMessage { get; set; }
        public Func<IPlayer, IPlayer?, Notification>? TeamAddMessage { get; set; }
        public Func<IPlayer, IPlayer, Notification>? TeamRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, IPlayer, Notification>? TeamChangeMessage { get; set; }

        public static TargetNotification Enum<T>(IAbility ability) where T : Enum
        {
            var type = typeof(T);
            var names = System.Enum.GetValues(type);

            Func<IPlayer, Notification>? userAdd = null;
            Func<IPlayer, Notification>? userRemove = null;
            Func<IPlayer, IPlayer, Notification>? userChange = null;
            Func<IPlayer, IPlayer, Notification>? targetAdd = null;
            Func<IPlayer, IPlayer, Notification>? targetRemove = null;
            Func<IPlayer, IPlayer, IPlayer, Notification>? targetChange = null;
            Func<IPlayer, IPlayer?, Notification>? teamAdd = null;
            Func<IPlayer, IPlayer, Notification>? teamRemove = null;
            Func<IPlayer, IPlayer, IPlayer, Notification>? teamChange = null;
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
                        targetAdd = (user, target) => Notification.Chat(role, key, user, target);
                        break;
                    case "TargetRemoveMessage":
                        targetRemove = (user, target) => Notification.Chat(role, key, user, target);
                        break;
                    case "TargetChangeMessage":
                        targetChange = (user, old, current) => Notification.Chat(role, key, user, old, current);
                        break;
                    case "TeamAddMessage":
                        teamAdd = (user, target) => Notification.Chat(role, key, user, target!);
                        break;
                    case "TeamRemoveMessage":
                        teamRemove = (user, target) => Notification.Chat(role, key, user, target);
                        break;
                    case "TeamChangeMessage":
                        teamChange = (user, old, current) => Notification.Chat(role, key, user, old, current);
                        break;
                }

            return new TargetNotification(
                userAdd,
                userRemove,
                userChange,
                targetAdd,
                targetRemove,
                targetChange,
                teamAdd,
                teamRemove,
                teamChange);
        }

        public Notification UserAdd(IPlayer target)
        {
            return (UserAddMessage ?? DefaultUser)(target);
        }

        public Notification UserRemove(IPlayer target)
        {
            return (UserRemoveMessage ?? DefaultUser)(target);
        }

        public Notification UserChange(IPlayer old, IPlayer current)
        {
            return (UserChangeMessage ?? DefaultChangeUser)(old, current);
        }

        public Notification TargetAdd(IPlayer user, IPlayer target)
        {
            return (TargetAddMessage ?? DefaultOther)(user, target);
        }

        public Notification TargetRemove(IPlayer user, IPlayer target)
        {
            return (TargetRemoveMessage ?? DefaultOther)(user, target);
        }

        public Notification TargetChange(IPlayer user, IPlayer old, IPlayer current)
        {
            return (TargetChangeMessage ?? DefaultChangeOther)(user, old, current);
        }

        public Notification TeamAdd(IPlayer user, IPlayer? target)
        {
            return (TeamAddMessage ?? DefaultOther)(user, target);
        }

        public Notification TeamRemove(IPlayer user, IPlayer target)
        {
            return (TeamRemoveMessage ?? DefaultOther)(user, target);
        }

        public Notification TeamChange(IPlayer user, IPlayer old, IPlayer current)
        {
            return (TeamChangeMessage ?? DefaultChangeOther)(user, old, current);
        }
    }
}