using JetBrains.Annotations;
using System;

namespace Mafia.NET.Players.Roles.Abilities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [BaseTypeRequired(typeof(IAbility))]
    [MeansImplicitUse]
    public sealed class RegisterAbilityAttribute : Attribute
    {
        public string Name { get; }
        public Type Setup { get; }

        public RegisterAbilityAttribute(string name, Type setup)
        {
            Name = name;
            Setup = setup;
        }
    }
}
