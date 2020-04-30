using Mafia.NET.Players;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    interface IChat
    {
        public IList<IChatParticipant> Participants { get; }
    }
}
