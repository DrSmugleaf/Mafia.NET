using System;
using JetBrains.Annotations;

namespace Mafia.NET.Players.Roles.HealProfiles;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[BaseTypeRequired(typeof(IHealProfile))]
[MeansImplicitUse]
public class RegisterHealAttribute : Attribute
{
    public RegisterHealAttribute(string id)
    {
        Id = id;
    }

    public string Id { get; }
}