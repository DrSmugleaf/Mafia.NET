using System.Collections.Concurrent;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Setups
{
    public class RoleSetupEntry : IRegistrable
    {
        public RoleSetupEntry(string id)
        {
            Id = id;
            Setups = new ConcurrentDictionary<AbilityEntry, AbilitySetupEntry>();
        }

        protected ConcurrentDictionary<AbilityEntry, AbilitySetupEntry> Setups { get; }

        public string Id { get; }

        public AbilitySetupEntry Ability(AbilityEntry entry)
        {
            return Setups.GetOrAdd(entry, new AbilitySetupEntry(entry));
        }

        public AbilitySetupEntry Ability<T>() where T : IAbility
        {
            return Ability(AbilityRegistry.Default[typeof(T)]);
        }
    }
}