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
        public RegisterAbilityAttribute(string id, int priority, [CanBeNull] Type setup = null)
        {
            Id = id;
            Priority = priority;

            if (setup != null && (setup.IsAbstract || setup.IsInterface))
                throw new ArgumentException($"Setup type {setup} is not a concrete class");

            Setup = setup;
        }

        public string Id { get; }
        public int Priority { get; }
        [CanBeNull] public Type Setup { get; }
    }
}