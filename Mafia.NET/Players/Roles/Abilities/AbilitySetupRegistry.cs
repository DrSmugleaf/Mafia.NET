using System;
using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class AbilitySetupRegistry
    {
        public AbilitySetupRegistry()
        {
            Types = new Dictionary<Type, IAbilitySetup>();
        }

        protected IDictionary<Type, IAbilitySetup> Types { get; set; }

        public T Setup<T>() where T : class, IAbilitySetup, new()
        {
            var type = typeof(T);

            if (!Types.TryGetValue(type, out var setup))
            {
                setup = new T();
                Types.Add(type, setup);
            }

            return setup as T;
        }

        public void Set(params IAbilitySetup[] setups)
        {
            foreach (var setup in setups)
            {
                var type = setup.GetType();
                Types.Add(type, setup);
            }
        }
    }
}