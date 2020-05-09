using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public interface IChatParticipant
    {
        IPlayer Owner { get; }
        string Name { get; set; }
        bool Muted { get; set; }
        bool Deaf { get; set; }
    }

    public class ChatParticipant : IChatParticipant
    {
        public IPlayer Owner { get; }
        public string Name { get; set; }
        public bool Muted { get; set; }
        public bool Deaf { get; set; }

        public ChatParticipant(IPlayer owner, string name, bool muted = false, bool deaf = false)
        {
            Owner = owner;
            Name = name;
            Muted = muted || owner.Blackmailed;
            Deaf = deaf;
        }

        public ChatParticipant(IPlayer owner, bool muted = false, bool deaf = false) : this(owner, owner.Name, muted, deaf)
        {
        }
    }
}
