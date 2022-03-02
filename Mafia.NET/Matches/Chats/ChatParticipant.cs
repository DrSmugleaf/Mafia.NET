using Mafia.NET.Localization;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats
{
    public interface IChatParticipant
    {
        IPlayer Owner { get; }
        Key? Nickname { get; set; }
        bool Muted { get; set; }
        bool Deaf { get; set; }

        Text DisplayName(IPlayer player);
        bool CanSend();
        bool CanReceive();
        void Pause(bool paused = true);
    }

    public class ChatParticipant : IChatParticipant
    {
        public ChatParticipant(IPlayer owner, Key? nickname, bool muted = false, bool deaf = false)
        {
            Owner = owner;
            Nickname = nickname;
            Muted = muted || owner.Perks.Blackmailed || !owner.Alive;
            Deaf = deaf;
        }

        public ChatParticipant(IPlayer owner, bool muted = false, bool deaf = false) :
            this(owner, null, muted, deaf)
        {
        }

        public bool Paused { get; set; }

        public IPlayer Owner { get; }
        public Key? Nickname { get; set; }
        public bool Muted { get; set; }
        public bool Deaf { get; set; }

        public Text DisplayName(IPlayer player)
        {
            return Nickname == null ? Owner.Name : Nickname.Localize(player.Culture);
        }

        public bool CanSend()
        {
            return !Muted && !Paused;
        }

        public bool CanReceive()
        {
            return !Deaf && !Paused;
        }

        public void Pause(bool paused = true)
        {
            Paused = paused;
        }
    }
}