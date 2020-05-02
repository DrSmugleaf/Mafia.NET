﻿namespace Mafia.NET.Matches.Phases
{
    class ConclusionPhase : BasePhase
    {
        public ConclusionPhase(int duration = 120) : base("Conclusion", duration)
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }

        public override IPhase End(IMatch match)
        {
            match.End();
            return this;
        }
    }
}
