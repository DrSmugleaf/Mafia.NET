using System;
using JetBrains.Annotations;

namespace Mafia.NET.Localization
{
    [AttributeUsage(AttributeTargets.Enum)]
    [BaseTypeRequired(typeof(Enum))]
    public sealed class RegisterKeyAttribute : Attribute
    {
    }
}