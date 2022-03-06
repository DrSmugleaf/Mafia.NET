using System.Globalization;

namespace Mafia.NET.Localization;

public interface ILocalizable
{
    Text Localize(CultureInfo? culture = null);
}