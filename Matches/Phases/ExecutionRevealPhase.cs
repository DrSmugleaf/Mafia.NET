﻿namespace Mafia.NET.Matches.Phases
{
    public class ExecutionRevealPhase : BasePhase
    {
        public ExecutionRevealPhase() : base("Execution Reveal", nextPhase: new NightPhase())
        {
        }

        public override void Start(IMatch match)
        {
            throw new System.NotImplementedException();
        }
    }
}
