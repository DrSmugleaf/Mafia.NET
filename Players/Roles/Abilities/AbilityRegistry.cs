using Mafia.NET.Matches;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilityRegistry
    {
        public class AbilityEntry
        {
            public string Name { get; }
            public Type Ability { get; }
            public Type Setup { get; }

            public AbilityEntry(string name, Type ability, Type setup)
            {
                Name = name;
                Ability = ability;
                Setup = setup;
            }
        }

        private static readonly Lazy<AbilityRegistry> Lazy = new Lazy<AbilityRegistry>(() => new AbilityRegistry());
        public static AbilityRegistry Instance { get => Lazy.Value; }
        public IImmutableDictionary<string, AbilityEntry> Names { get; }
        public IImmutableDictionary<Type, AbilityEntry> Types { get; }

        private AbilityRegistry()
        {
            var names = new Dictionary<string, AbilityEntry>();
            var types = new Dictionary<Type, AbilityEntry>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attribute = (RegisterAbilityAttribute)type.GetCustomAttributes(typeof(RegisterAbilityAttribute), true).FirstOrDefault();

                    if (attribute == null) continue;

                    var name = attribute.Name;
                    var ability = type;
                    var setup = attribute.Setup;
                    var entry = new AbilityEntry(name, ability, setup);

                    names[name] = entry;
                    types[type] = entry;
                }
            }

            Names = names.ToImmutableDictionary();
            Types = types.ToImmutableDictionary();
        }

        public T Ability<T>(IMatch match, IPlayer user) where T : IAbility
        {
            var type = typeof(T);

            if (!Types.ContainsKey(type))
            {
                throw new InvalidCastException("No ability found with type " + type);
            }

            return (T)Activator.CreateInstance(type, new object[] { match, user });
        }

        public T Setup<T>() where T : IAbility, new() => new T();
    }
}
