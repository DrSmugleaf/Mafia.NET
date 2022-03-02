using System;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Notifications
{
    public class Notification : ILocalizable
    {
        public static readonly Notification Empty = new Notification("", default);

        public Notification(string key, NotificationLocation location, params object[] args)
        {
            Key = new Key(key).Id;
            Location = location;
            Args = args;
        }

        public Notification(Key key, NotificationLocation location, params object[] args)
        {
            Key = key.Id;
            Location = location;
            Args = args;
        }

        public string Key { get; }
        public NotificationLocation Location { get; }
        public object[] Args { get; }

        public Text Localize(CultureInfo? culture = null)
        {
            return Localizer.Default.Get(Key, culture, Args);
        }

        public static Notification Chat(string key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Chat, args);
        }

        public static Notification Chat(Key key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Chat, args);
        }

        public static Notification Chat(IRole role, Enum key, params object[] args)
        {
            return Chat(new Key(role, key), args);
        }

        public static Notification Popup(string key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Popup, args);
        }

        public static Notification Popup(Key key, params object[] args)
        {
            return new Notification(key, NotificationLocation.Popup, args);
        }

        public static Notification Popup(IRole role, Enum key, params object[] args)
        {
            return Popup(new Key(role, key), args);
        }

        public override bool Equals(object? obj)
        {
            return obj is Notification o &&
                   Key.Equals(o.Key) &&
                   Location.Equals(o.Location) &&
                   Args.Equals(o.Args);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Location, Args);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}