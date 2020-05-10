using System;
using JetBrains.Annotations;

namespace Mafia.NET.Players.Roles.Abilities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [BaseTypeRequired(typeof(IAbility))]
    [MeansImplicitUse]
    public sealed class RegisterAbilityAttribute : Attribute
    {
        public RegisterAbilityAttribute(string name, Type setup)
        {
            Name = name;
            Setup = setup;
        }

        public string Name { get; }
        public Type Setup { get; }
    }
}