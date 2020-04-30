using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    public interface IChat
    {
        IList<IChatParticipant> Participants { get; }
    }
}
