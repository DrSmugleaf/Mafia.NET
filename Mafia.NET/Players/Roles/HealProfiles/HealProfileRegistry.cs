using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.HealProfiles
{
    public class HealProfileRegistry : ImmutableRegistry<HealProfileEntry>
    {
        private static readonly Lazy<HealProfileRegistry> Lazy = new(() => new HealProfileRegistry());

        public HealProfileRegistry(Dictionary<string, HealProfileEntry> ids) : base(ids)
        {
            Types = Ids.ToImmutableDictionary(
                pair => pair.Value.Profile,
                pair => pair.Value);
        }

        private HealProfileRegistry() : this(LoadAll())
        {
        }

        public static HealProfileRegistry Default => Lazy.Value;
        public ImmutableDictionary<Type, HealProfileEntry> Types { get; }

        public HealProfileEntry this[Type type] => Types[type];

        public static Dictionary<string, HealProfileEntry> LoadAll()
        {
            var ids = new Dictionary<string, HealProfileEntry>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
            {
                var profile = (RegisterHealAttribute?) type
                    .GetCustomAttributes(typeof(RegisterHealAttribute), true)
                    .FirstOrDefault();

                if (profile == null) continue;

                var id = profile.Id;
                var entry = new HealProfileEntry(id, type);

                if (ids.ContainsKey(id))
                    throw new ArgumentException($"Heal profile with id {id} is already registered.");

                ids[id] = entry;
            }

            return ids;
        }

        public HealProfileEntry Entry<T>() where T : IHealProfile
        {
            var type = typeof(T);

            if (!Types.TryGetValue(type, out var entry))
                throw new ArgumentException($"No heal profile found with {type}");

            return entry;
        }
    }
}