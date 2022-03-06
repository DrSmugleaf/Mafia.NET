using System;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Registry;

public class AbilityEntry<T> : IRegistrable where T : IAbility
{
    public AbilityEntry(
        string id,
        Type ability,
        int priority,
        Type? setup,
        MessageRandomizer murderDescriptions,
        int? defaultUses = null)
    {
        Id = id;

        if (!typeof(T).IsAssignableFrom(ability))
            throw new ArgumentException($"{ability} is not assignable to {typeof(T)}");

        Ability = ability;
        Priority = priority;
        Setup = setup ?? typeof(EmptySetup);
        MurderDescriptions = murderDescriptions;
        DefaultUses = defaultUses;
    }

    public Type Ability { get; }
    public int Priority { get; }
    public Type Setup { get; }
    public MessageRandomizer MurderDescriptions { get; }
    public int? DefaultUses { get; }
    public string Id { get; }

    public bool ValidSetup(IAbilitySetup setup)
    {
        var type = setup.GetType();
        return type == Setup || type.IsSubclassOf(Setup);
    }

    public AbilityEntry With(int? uses = null)
    {
        return new AbilityEntry(Id, Ability, Priority, Setup, MurderDescriptions, uses);
    }

    public T Build(IPlayer user)
    {
        var ability = (IAbility?) Activator.CreateInstance(Ability);
        if (ability == null) throw new NullReferenceException();
        var setup = user.Match.AbilitySetups[Id];

        ability.Initialize(setup, user);

        return (T) ability;
    }
}

public class AbilityEntry : AbilityEntry<IAbility>
{
    public AbilityEntry(
        string id,
        Type ability,
        int priority,
        Type? setup,
        MessageRandomizer murderDescriptions,
        int? defaultUses = null) :
        base(id, ability, priority, setup, murderDescriptions, defaultUses)
    {
    }
}