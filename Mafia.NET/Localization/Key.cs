using System;
using System.Globalization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Localization
{
    public class Key : ILocalizable
    {
        public static readonly Key Empty = new Key("");

        public Key(string key)
        {
            Id = key.ToLower().Replace(" ", "");
        }

        public Key(Enum key)
        {
            Id = (key.GetType().Name.ToLower().Replace("key", "") +
                  Enum.GetName(key.GetType(), key)).ToLower().Replace(" ", "");
        }

        public string Id { get; }
        
        public static implicit operator Key(Enum enumKey)
        {
            return new Key(enumKey);
        }

        public Notification Chat(params object[] args)
        {
            return Notification.Chat(Id, args);
        }

        public Notification Popup(params object[] args)
        {
            return Notification.Popup(Id, args);
        }

        public string Localize(CultureInfo culture = null)
        {
            return Localizer.Default.Get(this, culture);
        }

        public override string ToString()
        {
            return Localize();
        }
    }
}