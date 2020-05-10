using System;
using Mafia.NET.Matches;

namespace Mafia.NET.Players
{
    public class Note
    {
        public Note(IMatch match, IPlayer player)
        {
            Match = match;
            Owner = player;
            _text = "";
        }

        public IMatch Match { get; }
        public IPlayer Owner { get; }
        public string _text { get; set; }

        public string Text
        {
            get => _text;
            set
            {
                if (value == null) value = "";
                if (!Owner.Alive || !Match.Phase.CurrentPhase.Actionable) return;
                _text = value.Trim().Substring(0, Math.Min(value.Length, 500));
            }
        }

        public static implicit operator string(Note note)
        {
            return note.Text;
        }
    }
}