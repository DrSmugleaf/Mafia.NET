using System;
using System.Collections.Generic;
using Mafia.NET.Players.Roles.Abilities.Registry;

namespace Mafia.NET.Players.Roles.Abilities.Setups
{
    public class AbilitySetupRegistry
    {
        public AbilitySetupRegistry()
        {
            Types = new Dictionary<Type, IAbilitySetup>();
        }

        protected IDictionary<Type, IAbilitySetup> Types { get; set; }

        public IAbilitySetup Setup(Type type)
        {
            if (!Types.TryGetValue(type, out var setup))
            {
                setup = (IAbilitySetup) Activator.CreateInstance(type);
                Types[type] = setup;
            }

            return setup;
        }

        public IAbilitySetup Setup(AbilityEntry ability)
        {
            return Setup(ability.Setup);
        }

        public T Setup<T>() where T : class, IAbilitySetup, new()
        {
            return (T) Setup(typeof(T));
        }

        public void Set(params IAbilitySetup[] setups)
        {
            foreach (var setup in setups)
            {
                var type = setup.GetType();
                Types[type] = setup;
            }
        }
    }
}