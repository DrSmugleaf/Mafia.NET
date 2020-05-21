using System;
using Mafia.NET.Localization;

namespace Mafia.NET.Notifications
{
    public class Notification
    {
        public static readonly Notification Empty = new Notification("", default);

        public Notification(string key, NotificationLocation location, params object[] args)
        {
            Key = new Key(key).Id;
            Location = location;
            Args = args;
        }

        public Notification(Enum key, NotificationLocation location, params object[] args)
        {
            Key = new Key(key).Id;
            Location = location;
            Args = args;
        }

        public string Key { get; }
        public NotificationLocation Location { get; }
        public object[] Args { get; }

        public static Notification Chat(string key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Chat, args);
        }

        public static Notification Chat(Enum key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Chat, args);
        }

        public static Notification Popup(string key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Popup, args);
        }

        public static Notification Popup(Enum key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Popup, args);
        }
    }
}