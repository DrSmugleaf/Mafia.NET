using System;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetNotification
    {
        public static readonly TargetNotification Empty = new TargetNotification();
        public static readonly Func<IPlayer, string> Default = player => "";
        public static readonly Func<IPlayer, IPlayer, string> DefaultChange = (old, _new) => "";

        public TargetNotification(
            Func<IPlayer, string> userAdd = null,
            Func<IPlayer, string> userRemove = null,
            Func<IPlayer, IPlayer, string> userChange = null,
            Func<IPlayer, string> targetAdd = null,
            Func<IPlayer, string> targetRemove = null,
            Func<IPlayer, IPlayer, string> targetChange = null)
        {
            UserAddMessage = userAdd ?? Default;
            UserRemoveMessage = userRemove ?? Default;
            UserChangeMessage = userChange ?? DefaultChange;
            TargetAddMessage = targetAdd ?? Default;
            TargetRemoveMessage = targetRemove ?? Default;
            TargetChangeMessage = targetChange ?? DefaultChange;
        }

        public Func<IPlayer, string> UserAddMessage { get; set; }
        public Func<IPlayer, string> UserRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, string> UserChangeMessage { get; set; }
        public Func<IPlayer, string> TargetAddMessage { get; set; }
        public Func<IPlayer, string> TargetRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, string> TargetChangeMessage { get; set; }

        public Notification UserAdd(IPlayer target)
        {
            return Notification.Chat(UserAddMessage(target));
        }

        public Notification UserRemove(IPlayer target)
        {
            return Notification.Chat(UserRemoveMessage(target));
        }

        public Notification UserChange(IPlayer old, IPlayer _new)
        {
            return Notification.Chat(UserChangeMessage(old, _new));
        }

        public Notification TargetAdd(IPlayer target)
        {
            return Notification.Chat(TargetAddMessage(target));
        }

        public Notification TargetRemove(IPlayer target)
        {
            return Notification.Chat(TargetRemoveMessage(target));
        }

        public Notification TargetChange(IPlayer old, IPlayer current)
        {
            return Notification.Chat(TargetChangeMessage(old, current));
        }
    }
}