﻿using Mafia.NET.Matches.Chats;
using System;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetMessage
    {
        public static readonly TargetMessage Empty = new TargetMessage();
        public static readonly Func<IPlayer, string> Default = (player) => "";
        public static readonly Func<IPlayer, IPlayer, string> DefaultChange = (old, _new) => "";
        public Func<IPlayer, string> UserAddMessage { get; set; }
        public Func<IPlayer, string> UserRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, string> UserChangeMessage { get; set; }
        public Func<IPlayer, string> TargetAddMessage { get; set; }
        public Func<IPlayer, string> TargetRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, string> TargetChangeMessage { get; set; }

        public TargetMessage(
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

        public Notification UserAdd(IPlayer target) => Notification.Chat(UserAddMessage(target));

        public Notification UserRemove(IPlayer target) => Notification.Chat(UserRemoveMessage(target));

        public Notification UserChange(IPlayer old, IPlayer _new) => Notification.Chat(UserChangeMessage(old, _new));

        public Notification TargetAdd(IPlayer target) => Notification.Chat(TargetAddMessage(target));

        public Notification TargetRemove(IPlayer target) => Notification.Chat(TargetRemoveMessage(target));

        public Notification TargetChange(IPlayer old, IPlayer _new) => Notification.Chat(TargetChangeMessage(old, _new));
    }
}