using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    class Chat : IChat
    {
        public IList<IChatParticipant> Participants { get; }

        public Chat(IList<IChatParticipant> participants)
        {
            Participants = participants;
        }
    }
}
