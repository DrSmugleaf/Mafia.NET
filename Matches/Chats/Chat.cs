using System.Collections.Generic;

namespace Mafia.NET.Matches.Chats
{
    public class Chat : IChat
    {
        public IList<IChatParticipant> Participants { get; }

        public Chat(IList<IChatParticipant> participants)
        {
            Participants = participants;
        }
    }
}
