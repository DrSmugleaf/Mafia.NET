using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;

namespace Mafia.NET.Players.Targeting
{
    public class TargetManager
    {
        public TargetManager(IPlayer user, IReadOnlyDictionary<Time, PhaseTargeting> phases)
        {
            User = user;
            Phases = phases;
        }

        public TargetManager(IPlayer user)
        {
            User = user;

            var phases = new Dictionary<Time, PhaseTargeting>();

            foreach (Time phase in Enum.GetValues(typeof(Time)))
                phases[phase] = new PhaseTargeting(user, phase);

            Phases = phases;
        }

        public IPlayer User { get; }
        public IMatch Match => User.Match;
        public Time CurrentTime => Match.Phase.CurrentTime;
        public IReadOnlyDictionary<Time, PhaseTargeting> Phases { get; }

        [CanBeNull]
        public IPlayer this[Time phase, int index]
        {
            get => Phases[phase][index]?.Targeted;
            set => Phases[phase][index]?.Set(value);
        }

        [CanBeNull]
        public IPlayer this[int index]
        {
            get => Get()[index]?.Targeted;
            set => Get()[index]?.Set(value);
        }

        public bool Any(IPlayer targeted)
        {
            return Get().Targets.Any(target => target.Targeted == targeted);
        }

        public PhaseTargeting Get(Time time)
        {
            return Phases[time];
        }

        public PhaseTargeting Get()
        {
            return Get(CurrentTime);
        }

        public PhaseTargeting GetAll(Time time)
        {
            return Get(time);
        }

        public PhaseTargeting GetAll()
        {
            return GetAll(CurrentTime);
        }

        [CanBeNull]
        public IPlayer Day(int index = 0)
        {
            return this[Time.Day, index];
        }

        [CanBeNull]
        public IPlayer Night(int index = 0)
        {
            return this[Time.Night, index];
        }

        public bool Try(Time phase, int index, [MaybeNullWhen(false)] out IPlayer target)
        {
            target = this[phase, index];
            return target != null;
        }

        public bool Try(int index, [MaybeNullWhen(false)] out IPlayer target)
        {
            return Try(CurrentTime, index, out target);
        }

        public bool Try([MaybeNullWhen(false)] out IPlayer target)
        {
            return Try(0, out target);
        }

        public bool TryDay(int index, [MaybeNullWhen(false)] out IPlayer target)
        {
            return Try(Time.Day, index, out target);
        }

        public bool TryDay([MaybeNullWhen(false)] out IPlayer target)
        {
            return TryDay(0, out target);
        }

        public bool TryNight(int index, [MaybeNullWhen(false)] out IPlayer target)
        {
            return Try(Time.Night, index, out target);
        }

        public bool TryNight([MaybeNullWhen(false)] out IPlayer target)
        {
            return TryNight(0, out target);
        }

        public void Add(Target target)
        {
            Get().Add(target);
        }

        public void Add(TargetNotification message, params IPlayer[] targets)
        {
            Get().Add(message, targets);
        }

        public void Set([CanBeNull] IPlayer target)
        {
            Get().Set(target);
        }

        public void ForceSet([CanBeNull] IPlayer target)
        {
            Get().ForceSet(target);
        }

        public void Reset()
        {
            Get().Reset();
        }

        public void Reset(Time time)
        {
            Phases[time].Reset();
        }

        public void Reset(Target target)
        {
            Get().Reset(target);
        }

        public void Reset(TargetNotification message, params IPlayer[] targets)
        {
            Get().Reset(message, targets);
        }
    }
}