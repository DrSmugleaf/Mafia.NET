using Mafia.NET.Matches;
using System;

namespace Mafia.NET.Players
{
    public class Note
    {
        public IMatch Match { get; }
        public IPlayer Owner { get; }
        public string _text { get; set; }
        public string Text
        {
            get => _text;
            set
            {
                if (!Owner.Alive || !Match.Phase.CurrentPhase.Actionable) return;
                _text = value.Trim().Substring(0, Math.Min(value.Length, 500));
            }
        }

        public Note(IMatch match, IPlayer player)
        {
            Match = match;
            Owner = player;
            _text = "";
        }

        public static implicit operator string(Note note) => note.Text;
    }
}
