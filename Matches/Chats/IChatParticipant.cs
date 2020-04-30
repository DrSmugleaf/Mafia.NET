using Mafia.NET.Players;

#nullable enable

namespace Mafia.NET.Matches.Chats
{
    public interface IChatParticipant
    {
        IPlayer Owner { get; }
        string? DisplayName { get; set; }
        bool Muted { get; set; }
        bool Deaf { get; set; }
    }
}
