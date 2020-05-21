using System;
using System.Globalization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Localization
{
    public class Key
    {
        public static readonly Key Empty = new Key("");

        public Key(string key)
        {
            Id = key.ToLower();
        }

        public Key(Enum key)
        {
            Id = (key.GetType().Name.ToLower().Replace("key", "") + 
                  Enum.GetName(key.GetType(), key)).ToLower();
        }

        public string Id { get; }

        public Notification Chat(params object[] args)
        {
            return Notification.Chat(Id, args);
        }

        public Notification Popup(params object[] args)
        {
            return Notification.Popup(Id, args);
        }

        public string ToString(CultureInfo culture)
        {
            return Localizer.Default.Get(this, culture);
        }
    }
}