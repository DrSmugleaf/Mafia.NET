using System;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Abilities.Setups
{
    public class AbilitySetupEntry : IRegistrable
    {
        private IAbilitySetup? _setup;

        public AbilitySetupEntry(AbilityEntry ability)
        {
            Id = ability.Id;
            Ability = ability;
        }

        public AbilityEntry Ability { get; set; }

        public IAbilitySetup? Setup
        {
            private get => _setup;
            set
            {
                if (value == null || !Ability.ValidSetup(value))
                    throw new ArgumentException($"Invalid setup {value?.GetType()}");

                _setup = value;
            }
        }

        public int? Uses { private get; set; }

        public string Id { get; }

        public IAbilitySetup ResolveSetup()
        {
            return (IAbilitySetup) (Setup ?? Activator.CreateInstance(Ability.Setup)!);
        }

        public int ResolveUses(IRole role)
        {
            int? abilityDefault = null;
            if (RoleRegistry.Default[role.Id].Abilities.TryGetValue(Id, out var ability))
                abilityDefault = ability.DefaultUses;

            return Uses ?? abilityDefault ?? 0;
        }
    }
}