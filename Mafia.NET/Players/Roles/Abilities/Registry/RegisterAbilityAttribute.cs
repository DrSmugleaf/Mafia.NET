using System;
using JetBrains.Annotations;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Roles.Abilities.Registry
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [BaseTypeRequired(typeof(IAbility))]
    [MeansImplicitUse]
    public class RegisterAbilityAttribute : Attribute
    {
        public RegisterAbilityAttribute(string id, int priority, [CanBeNull] Type defaultSetup = null)
        {
            Id = id;
            Priority = priority;

            if (defaultSetup != null && (defaultSetup.IsAbstract || defaultSetup.IsInterface))
                throw new ArgumentException($"Setup type {defaultSetup} is not a concrete class");

            DefaultSetup = defaultSetup;
        }

        public string Id { get; }
        public int Priority { get; }
        [CanBeNull] public Type DefaultSetup { get; }
    }
}