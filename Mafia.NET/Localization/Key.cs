using System;
using System.Collections.Generic;
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
                  System.Enum.GetName(key.GetType(), key)).ToLower().Replace(" ", "");
        }

        public string Id { get; }

        public string Localize(CultureInfo culture = null)
        {
            return Localizer.Default.Get(this, culture);
        }

        public override bool Equals(object obj)
        {
            if (obj is Key o)
                return Id.Equals(o.Id);
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static List<Key> Enum<T>() where T : Enum
        {
            var list = new List<Key>();
            var type = typeof(T);

            foreach (T value in System.Enum.GetValues(type))
            {
                var key = new Key(value);
                list.Add(key);
            }

            return list;
        }

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

        public override string ToString()
        {
            return Localize();
        }
    }
}