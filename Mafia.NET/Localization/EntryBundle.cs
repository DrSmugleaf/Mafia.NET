using System;
using System.Collections.Generic;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Localization
{
    public class EntryBundle
    {
        public EntryBundle()
        {
            Entries = new List<Notification>();
        }

        public List<Notification> Entries { get; }

        public EntryBundle Add(string key, NotificationLocation location, params object[] args)
        {
            var entry = new Notification(key, location, args);
            Entries.Add(entry);

            return this;
        }

        public EntryBundle Add(Key key, NotificationLocation location, params object[] args)
        {
            var entry = new Notification(key, location, args);
            Entries.Add(entry);

            return this;
        }

        public EntryBundle Add(IRole role, Enum entry, NotificationLocation location, params object[] args)
        {
            var key = new Key(role, entry);
            var notification = new Notification(key, location, args);
            Entries.Add(notification);

            return this;
        }

        public EntryBundle Chat(string key, params object[] args)
        {
            return Add(key, NotificationLocation.Chat, args);
        }

        public EntryBundle Chat(Key key, params object[] args)
        {
            return Add(key, NotificationLocation.Chat, args);
        }

        public EntryBundle Chat(IRole role, Enum key, params object[] args)
        {
            return Add(role, key, NotificationLocation.Chat, args);
        }

        public EntryBundle Popup(string key, params object[] args)
        {
            return Add(key, NotificationLocation.Popup, args);
        }

        public EntryBundle Popup(Key key, params object[] args)
        {
            return Add(key, NotificationLocation.Popup, args);
        }

        public EntryBundle Popup(IRole role, Enum key, params object[] args)
        {
            return Add(role, key, NotificationLocation.Popup, args);
        }
    }
}