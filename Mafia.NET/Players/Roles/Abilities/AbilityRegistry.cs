using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilityEntry
    {
        public AbilityEntry(string id, Type ability, Type setup, MessageRandomizer murderDescriptions)
        {
            Id = id;
            Ability = ability;
            Setup = setup;
            MurderDescriptions = murderDescriptions;
        }

        public string Id { get; }
        public Type Ability { get; }
        public Type Setup { get; }
        public MessageRandomizer MurderDescriptions { get; }

        public IAbility Build()
        {
            var ability = (IAbility) Activator.CreateInstance(Ability);
            if (ability == null) throw new NullReferenceException();

            ability.Name = Id;
            ability.MurderDescriptions = MurderDescriptions;

            return ability;
        }
    }

    public class AbilityRegistry
    {
        private static readonly MessageRandomizer DefaultMurderDescriptions =
            new MessageRandomizer("They died in mysterious ways");

        private static readonly Lazy<AbilityRegistry> Lazy = new Lazy<AbilityRegistry>(() => new AbilityRegistry());

        private AbilityRegistry()
        {
            var attributes = new Dictionary<string, (Type, RegisterAbilityAttribute)>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
            {
                var attribute =
                    (RegisterAbilityAttribute) type.GetCustomAttributes(typeof(RegisterAbilityAttribute), true)
                        .FirstOrDefault();

                if (attribute == null) continue;

                attributes[attribute.Id] = (type, attribute);
            }

            var names = new Dictionary<string, AbilityEntry>();
            var types = new Dictionary<Type, AbilityEntry>();
            var roles = Resource.FromDirectory("Roles", "*.yml");
            foreach (YamlMappingNode yaml in roles)
            {
                var id = yaml["id"].AsString();
                if (!attributes.ContainsKey(id)) continue; // TODO: Remove after all abilities are done

                var type = attributes[id].Item1;
                var ability = type;
                var setup = attributes[id].Item2.Setup;
                var murderDescriptions = DefaultMurderDescriptions;
                if (yaml.Try("ability", out var abilityYaml) &&
                    abilityYaml.Try("murder_descriptions", out var node))
                {
                    var descriptions = (YamlSequenceNode) node;
                    murderDescriptions = new MessageRandomizer(descriptions.AsStringList());
                }

                var entry = new AbilityEntry(id, ability, setup, murderDescriptions);

                if (names.ContainsKey(id))
                    throw new ArgumentException($"Ability with id {id} is already registered.");

                if (types.ContainsKey(type))
                    throw new ArgumentException($"Ability with type {type} is already registered.");

                names[id] = entry;
                types[type] = entry;
            }

            Names = names.ToImmutableDictionary();
            Types = types.ToImmutableDictionary();
        }

        public static AbilityRegistry Default => Lazy.Value;
        public IImmutableDictionary<string, AbilityEntry> Names { get; }
        public IImmutableDictionary<Type, AbilityEntry> Types { get; }

        public AbilityEntry Entry<T>() where T : IAbility
        {
            var type = typeof(T);

            if (!Types.ContainsKey(type)) throw new InvalidCastException("No ability found with type " + type);

            return Types[type];
        }

        public T Ability<T>() where T : IAbility, new()
        {
            var entry = Entry<T>();
            var ability = new T
            {
                Name = entry.Id,
                MurderDescriptions = entry.MurderDescriptions
            };

            return ability;
        }
    }
}