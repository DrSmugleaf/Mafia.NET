using Mafia.NET.Players;

#nullable enable

namespace Mafia.NET.Matches.Chats
{
    interface IChatParticipant
    {
        public IPlayer Owner { get; }
        public string? DisplayName { get; set; }
    }
}
