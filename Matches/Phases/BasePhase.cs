#nullable enable

using System;

namespace Mafia.NET.Matches.Phases
{
    public abstract class BasePhase : IPhase
    {
        public string Name { get; }
        public IPhase? NextPhase { get; }
        public bool Skippable { get; }
        public IPhase? Supersedes { get; set; }
        public IPhase? SupersededBy { get; set; }
        public event EventHandler<PhaseChangeEventArgs>? PhaseChanged;

        public BasePhase(string name, IPhase? nextPhase = null, bool skippable = false)
        {
            Name = name;
            NextPhase = nextPhase;
            Skippable = skippable;
        }

        public abstract void Start();
        public abstract void End();
        protected virtual void OnPhaseChange(PhaseChangeEventArgs e)
        {
            PhaseChanged?.Invoke(this, e);
        }
    }
}
