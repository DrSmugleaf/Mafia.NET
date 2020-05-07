using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilityEntry
    {
        public string Name { get; }
        public Type Ability { get; }
        public Type Setup { get; }
        public MessageRandomizer MurderDescriptions { get; } // TODO

        public AbilityEntry(string name, Type ability, Type setup)
        {
            Name = name;
            Ability = ability;
            Setup = setup;
        }
    }

    public class AbilityRegistry
    {

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

        public AbilityEntry Entry<T>() where T : IAbility
        {
            var type = typeof(T);

            if (!Types.ContainsKey(type))
            {
                throw new InvalidCastException("No ability found with type " + type);
            }

            return Types[type];
        }

        public T Ability<T>(IMatch match, IPlayer user) where T : IAbility
        {
            var entry = Entry<T>();
            return (T)Activator.CreateInstance(typeof(T), new object[] { match, user });
        }

        public T Setup<T>() where T : IAbilitySetup, new() => new T();
    }
}
