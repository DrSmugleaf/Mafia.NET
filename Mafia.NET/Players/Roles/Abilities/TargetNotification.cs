using System;
using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetNotification
    {
        public static readonly TargetNotification Empty = new TargetNotification();
        public static readonly Func<IPlayer, Entry> Default = player => Entry.Empty;
        public static readonly Func<IPlayer, IPlayer, Entry> DefaultChange = (old, current) => Entry.Empty;

        public TargetNotification(
            Func<IPlayer, Entry> userAdd = null,
            Func<IPlayer, Entry> userRemove = null,
            Func<IPlayer, IPlayer, Entry> userChange = null,
            Func<IPlayer, Entry> targetAdd = null,
            Func<IPlayer, Entry> targetRemove = null,
            Func<IPlayer, IPlayer, Entry> targetChange = null)
        {
            UserAddMessage = userAdd ?? Default;
            UserRemoveMessage = userRemove ?? Default;
            UserChangeMessage = userChange ?? DefaultChange;
            TargetAddMessage = targetAdd ?? Default;
            TargetRemoveMessage = targetRemove ?? Default;
            TargetChangeMessage = targetChange ?? DefaultChange;
        }

        public Func<IPlayer, Entry> UserAddMessage { get; set; }
        public Func<IPlayer, Entry> UserRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, Entry> UserChangeMessage { get; set; }
        public Func<IPlayer, Entry> TargetAddMessage { get; set; }
        public Func<IPlayer, Entry> TargetRemoveMessage { get; set; }
        public Func<IPlayer, IPlayer, Entry> TargetChangeMessage { get; set; }

        public static TargetNotification Enum<T>() where T : Enum
        {
            var type = typeof(T);
            var names = System.Enum.GetValues(type);

            Func<IPlayer, Entry> userAdd = null;
            Func<IPlayer, Entry> userRemove = null;
            Func<IPlayer, IPlayer, Entry> userChange = null;
            Func<IPlayer, Entry> targetAdd = null;
            Func<IPlayer, Entry> targetRemove = null;
            Func<IPlayer, IPlayer, Entry> targetChange = null;
            foreach (T key in names)
            {
                switch (key.ToString())
                {
                    case "UserAddMessage":
                        userAdd = target => Entry.Chat(key, target);
                        break;
                    case "UserRemoveMessage":
                        userRemove = target => Entry.Chat(key, target);
                        break;
                    case "UserChangeMessage":
                        userChange = (old, current) => Entry.Chat(key, old, current);
                        break;
                    case "TargetAddMessage":
                        targetAdd = target => Entry.Chat(key, target);
                        break;
                    case "TargetRemoveMessage":
                        targetRemove = target => Entry.Chat(key, target);
                        break;
                    case "TargetChangeMessage":
                        targetChange = (old, current) => Entry.Chat(key, old, current);
                        break;
                }
            }

            return new TargetNotification(userAdd, userRemove, userChange, targetAdd, targetRemove, targetChange);
        }

        public Entry UserAdd(IPlayer target)
        {
            return UserAddMessage(target);
        }

        public Entry UserRemove(IPlayer target)
        {
            return UserRemoveMessage(target);
        }

        public Entry UserChange(IPlayer old, IPlayer current)
        {
            return UserChangeMessage(old, current);
        }

        public Entry TargetAdd(IPlayer target)
        {
            return TargetAddMessage(target);
        }

        public Entry TargetRemove(IPlayer target)
        {
            return TargetRemoveMessage(target);
        }

        public Entry TargetChange(IPlayer old, IPlayer current)
        {
            return TargetChangeMessage(old, current);
        }
    }
}