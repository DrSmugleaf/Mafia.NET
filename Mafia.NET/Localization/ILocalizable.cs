using System.Globalization;
using JetBrains.Annotations;

namespace Mafia.NET.Localization
{
    public interface ILocalizable
    {
        Text Localize([CanBeNull] CultureInfo culture = null);
    }
}