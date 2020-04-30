using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases
{
    public class PhaseChangeEventArgs : EventArgs
    {
        public IPhase Before { get; set; }
        public IPhase After { get; set; }
        public IList<string> ChatMessages { get; set; }

        public PhaseChangeEventArgs(IPhase before, IPhase after)
        {
            Before = before;
            After = after;
        }

    }
}
