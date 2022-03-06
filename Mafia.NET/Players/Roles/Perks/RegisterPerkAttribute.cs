using System;
using JetBrains.Annotations;

namespace Mafia.NET.Players.Roles.Perks;

[AttributeUsage(AttributeTargets.Property)]
[MeansImplicitUse]
public class RegisterPerkAttribute : Attribute
{
}