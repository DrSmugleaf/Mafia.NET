using System.Globalization;
using JetBrains.Annotations;

namespace Mafia.NET.Localization
{
    public interface ILocalizable
    {
        string Localize([CanBeNull] CultureInfo culture = null);
    }
}