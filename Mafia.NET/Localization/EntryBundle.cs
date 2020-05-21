using System;
using System.Collections.Generic;

namespace Mafia.NET.Localization
{
    public class EntryBundle
    {
        public EntryBundle()
        {
            Entries = new List<Entry>();
        }

        public List<Entry> Entries { get; }

        public EntryBundle Add(string key, NotificationLocation location, params object[] args)
        {
            var entry = new Entry(key, location, args);
            Entries.Add(entry);

            return this;
        }

        public EntryBundle Add(Enum key, NotificationLocation location, params object[] args)
        {
            var entry = new Entry(key, location, args);
            Entries.Add(entry);

            return this;
        }

        public EntryBundle Chat(string key, params object[] args)
        {
            return Add(key, NotificationLocation.Chat, args);
        }

        public EntryBundle Chat(Enum key, params object[] args)
        {
            return Add(key, NotificationLocation.Chat, args);
        }

        public EntryBundle Popup(string key, params object[] args)
        {
            return Add(key, NotificationLocation.Popup, args);
        }

        public EntryBundle Popup(Enum key, params object[] args)
        {
            return Add(key, NotificationLocation.Popup, args);
        }
    }
}