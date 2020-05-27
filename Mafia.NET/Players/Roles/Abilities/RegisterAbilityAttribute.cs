using System;
using JetBrains.Annotations;

namespace Mafia.NET.Players.Roles.Abilities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [BaseTypeRequired(typeof(IAbility))]
    [MeansImplicitUse]
    public sealed class RegisterAbilityAttribute : Attribute
    {
        public RegisterAbilityAttribute(string id, Type setup)
        {
            Id = id;
            Setup = setup;
        }

        public string Id { get; }
        public Type Setup { get; }
    }
}