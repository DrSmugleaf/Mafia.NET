using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Registry;

public class AbilityRegistry : ImmutableRegistry<AbilityEntry>
{
    private static readonly MessageRandomizer DefaultMurderDescriptions = new("They died in mysterious ways");

    private static readonly Lazy<AbilityRegistry> Lazy = new(() => new AbilityRegistry());

    public AbilityRegistry(Dictionary<string, AbilityEntry> ids) : base(ids)
    {
        Types = Ids.ToImmutableDictionary(
            pair => pair.Value.Ability,
            pair => pair.Value);
    }

    private AbilityRegistry() : this(LoadAll())
    {
    }

    public static AbilityRegistry Default => Lazy.Value;
    public ImmutableDictionary<Type, AbilityEntry> Types { get; }

    public AbilityEntry this[Type type] => Types[type];

    public static Dictionary<string, AbilityEntry> LoadAll()
    {
        var ids = new Dictionary<string, AbilityEntry>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        foreach (var type in assembly.GetTypes())
        {
            var ability = (RegisterAbilityAttribute?) type
                .GetCustomAttributes(typeof(RegisterAbilityAttribute), true)
                .FirstOrDefault();

            if (ability == null) continue;

            var id = ability.Id;
            var priority = ability.Priority;
            var setup = ability.DefaultSetup;
            var murderDescriptions = DefaultMurderDescriptions;
            var entry = new AbilityEntry(id, type, priority, setup, murderDescriptions);

            if (ids.ContainsKey(id))
                throw new ArgumentException($"Ability with id {id} is already registered.");

            ids[id] = entry;
        }

        return ids;
    }

    public AbilityEntry Entry<T>() where T : IAbility
    {
        var type = typeof(T);

        if (!Types.TryGetValue(type, out var entry))
            throw new ArgumentException($"No ability found with type {type}");

        return entry;
    }

    public T Ability<T>(IPlayer user) where T : IAbility, new()
    {
        var entry = Entry<T>();
        return (T) entry.Build(user);
    }

    public List<AbilityEntry> Entries(IAbilitySetup setup)
    {
        return Ids.Values.Where(entry => entry.Setup == setup.GetType()).ToList();
    }
}