using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilityEntry
    {
        public AbilityEntry(
            string id,
            Type ability,
            int priority,
            [CanBeNull] Type setup,
            MessageRandomizer murderDescriptions)
        {
            Id = id;
            Ability = ability;
            Priority = priority;
            Setup = setup ?? typeof(EmptySetup);
            MurderDescriptions = murderDescriptions;
        }

        public string Id { get; }
        public Type Ability { get; }
        public int Priority { get; }
        public Type Setup { get; }
        public MessageRandomizer MurderDescriptions { get; }

        public bool ValidSetup(IAbilitySetup setup)
        {
            var type = setup.GetType();
            return type == Setup || type.IsSubclassOf(Setup);
        }

        public IAbility Build(IPlayer user)
        {
            var ability = (IAbility) Activator.CreateInstance(Ability);
            if (ability == null) throw new NullReferenceException();

            ability.Initialize(this, user);

            return ability;
        }
    }

    public class AbilityRegistry
    {
        private static readonly MessageRandomizer DefaultMurderDescriptions =
            new MessageRandomizer("They died in mysterious ways");

        public AbilityRegistry()
        {
            var ids = new Dictionary<string, AbilityEntry>();
            var types = new Dictionary<Type, AbilityEntry>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
            {
                var ability = (RegisterAbilityAttribute) type
                    .GetCustomAttributes(typeof(RegisterAbilityAttribute), true)
                    .FirstOrDefault();

                if (ability == null) continue;

                var id = ability.Id;
                var priority = ability.Priority;
                var setup = ability.Setup;
                var murderDescriptions = DefaultMurderDescriptions;
                var entry = new AbilityEntry(id, type, priority, setup, murderDescriptions);

                if (ids.ContainsKey(id))
                    throw new ArgumentException($"Ability with id {id} is already registered.");

                if (types.ContainsKey(type))
                    throw new ArgumentException($"Ability with type {type} is already registered.");

                ids[id] = entry;
                types[type] = entry;
            }

            Ids = ids.ToImmutableDictionary();
            Types = types.ToImmutableDictionary();
        }

        public IImmutableDictionary<string, AbilityEntry> Ids { get; }
        public IImmutableDictionary<Type, AbilityEntry> Types { get; }

        public AbilityEntry Entry<T>() where T : IAbility
        {
            var type = typeof(T);

            if (!Types.ContainsKey(type))
                throw new ArgumentException("No ability found with type " + type);

            return Types[type];
        }

        public T Ability<T>(IPlayer user) where T : IAbility, new()
        {
            var entry = Entry<T>();
            return (T) entry.Build(user);
        }

        public IList<IAbility> Abilities(IPlayer user, IEnumerable<string> ids)
        {
            var abilities = new List<IAbility>();

            foreach (var id in ids)
            {
                var entry = Ids[id];
                var ability = entry.Build(user);
                abilities.Add(ability);
            }

            return abilities;
        }
    }
}