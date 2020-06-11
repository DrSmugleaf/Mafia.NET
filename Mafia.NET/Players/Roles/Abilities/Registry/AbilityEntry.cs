using System;
using JetBrains.Annotations;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Registry
{
    public class AbilityEntry : IRegistrable
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

        public Type Ability { get; }
        public int Priority { get; }
        public Type Setup { get; }
        public MessageRandomizer MurderDescriptions { get; }

        public string Id { get; }

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
}