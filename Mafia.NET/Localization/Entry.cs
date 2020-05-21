using System;

namespace Mafia.NET.Localization
{
    public class Entry
    {
        public static readonly Entry Empty = new Entry("", default);

        public Entry(string key, NotificationLocation location, params object[] args)
        {
            Key = key;
            Location = location;
            Args = args;
        }

        public Entry(Enum key, NotificationLocation location, params object[] args)
        {
            Key = key.GetType().Name.ToLower().Replace("key", "") +
                  Enum.GetName(key.GetType(), key);
            Location = location;
            Args = args;
        }

        public string Key { get; }
        public NotificationLocation Location { get; }
        public object[] Args { get; }

        public static Entry Chat(string key, params object[] args)
        {
            return new Entry(key, NotificationLocation.Chat, args);
        }

        public static Entry Chat(Enum key, params object[] args)
        {
            return new Entry(key, NotificationLocation.Chat, args);
        }

        public static Entry Popup(string key, params object[] args)
        {
            return new Entry(key, NotificationLocation.Popup, args);
        }

        public static Entry Popup(Enum key, params object[] args)
        {
            return new Entry(key, NotificationLocation.Popup, args);
        }
    }
}