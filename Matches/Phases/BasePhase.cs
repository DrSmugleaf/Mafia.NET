#nullable enable

using System;

namespace Mafia.NET.Matches.Phases
{
    public abstract class BasePhase : IPhase
    {
        public string Name { get; }
        public int Duration { get; }
        public IPhase? NextPhase { get; }
        public bool Skippable { get; }
        public IPhase? Supersedes { get; set; }
        public IPhase? SupersededBy { get; set; }
        public event EventHandler<PhaseChangeEventArgs>? PhaseChanged;

        public BasePhase(string name, int duration = -1, IPhase? nextPhase = null, bool skippable = false)
        {
            Name = name;
            Duration = duration;
            NextPhase = nextPhase;
            Skippable = skippable;
        }

        public abstract void Start(IMatch match);
        public abstract void End(IMatch match);
        protected virtual void OnPhaseChange(PhaseChangeEventArgs e)
        {
            PhaseChanged?.Invoke(this, e);
        }
    }
}
