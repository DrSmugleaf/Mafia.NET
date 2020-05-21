using Mafia.NET.Notifications;

namespace Mafia.NET.Localization
{
    public interface INameable
    {
        string Name { get; }
        Key NameKey { get; }
    }
}