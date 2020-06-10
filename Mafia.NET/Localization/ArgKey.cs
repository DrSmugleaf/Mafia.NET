using System;
using System.Globalization;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Localization
{
    public class ArgKey : Key
    {
        public ArgKey(string key, params object[] args) : base(key)
        {
            Args = args;
        }

        public ArgKey(IRole role, Enum key, params object[] args) :
            base(role.Id + System.Enum.GetName(key.GetType(), key))
        {
            Args = args;
        }

        public object[] Args { get; }

        public override Text Localize(CultureInfo culture = null)
        {
            return Localizer.Default.Get(Id, culture, Args);
        }

        public override bool Equals(object obj)
        {
            return obj is ArgKey o &&
                   Id.Equals(o.Id) &&
                   Args.Equals(o.Args);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Args);
        }
    }
}