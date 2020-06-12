using System.Collections.Generic;
using JetBrains.Annotations;
using Mafia.NET.Matches.Phases;

namespace Mafia.NET.Players.Targeting
{
    public class PhaseTargeting
    {
        public PhaseTargeting(IPlayer user, Time phase, IList<Target> targets)
        {
            User = User;
            Phase = phase;
            Targets = targets;
        }

        public PhaseTargeting(IPlayer user, Time phase) : this(user, phase, new List<Target>())
        {
        }

        public IPlayer User { get; }
        public Time Phase { get; }
        public IList<Target> Targets { get; set; }

        [CanBeNull]
        public Target this[int index]
        {
            get => index < Targets.Count ? Targets[index] : null;
            set
            {
                if (value == null) return;
                Targets[index] = value;
            }
        }

        public void Add(Target target)
        {
            Targets.Add(target);
        }

        public void Set([CanBeNull] IPlayer target)
        {
            if (Targets.Count == 0) return;
            Targets[0].Targeted = target;
        }

        public void ForceSet([CanBeNull] IPlayer target)
        {
            if (Targets.Count == 0) return;
            Targets[0].ForceSet(target);
        }

        public void Reset()
        {
            Targets.Clear();
        }

        public void Reset(Target target)
        {
            Reset();
            Targets.Add(target);
        }
    }
}