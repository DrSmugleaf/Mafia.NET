using System;
using JetBrains.Annotations;

namespace Mafia.NET.Players.Roles.Abilities.Setups;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
[BaseTypeRequired(typeof(IAbilitySetup))]
[MeansImplicitUse]
public class RegisterSetupAttribute : Attribute
{
}