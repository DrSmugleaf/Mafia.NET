using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetManager
    {
        public TargetManager(IMatch match, IAbility ability, IReadOnlyDictionary<Time, PhaseTargeting> phases)
        {
            Match = match;
            Ability = ability;
            Phases = phases;
        }

        public TargetManager(IMatch match, IAbility ability)
        {
            Match = match;
            Ability = ability;

            var phases = new Dictionary<Time, PhaseTargeting>();

            foreach (Time phase in Enum.GetValues(typeof(Time))) phases[phase] = new PhaseTargeting(phase);

            Phases = phases;
        }

        public IMatch Match { get; }
        public IAbility Ability { get; }
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

        public PhaseTargeting Get()
        {
            return Phases[Match.Phase.CurrentTime];
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
            return Try(Match.Phase.CurrentTime, index, out target);
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
            Get().Add(Ability, message, targets);
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
            Get().Reset(Ability, message, targets);
        }
    }

    public class PhaseTargeting
    {
        public PhaseTargeting(Time phase, IList<Target> targets)
        {
            Phase = phase;
            Targets = targets;
        }

        public PhaseTargeting(Time phase) : this(phase, new List<Target>())
        {
        }

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

        public void Add(IAbility ability, [CanBeNull] TargetNotification message = null, params IPlayer[] targets)
        {
            Add(TargetFilter.Of(targets).Build(ability, message));
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

        public void Reset(IAbility ability, TargetNotification message, params IPlayer[] targets)
        {
            Reset(TargetFilter.Of(targets).Build(ability, message));
        }
    }

    public class TargetFilter
    {
        public static readonly TargetFilter Any = new TargetFilter(players => players);

        private readonly Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> _filter;

        private TargetFilter(Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> filter)
        {
            _filter = filter;
        }

        private TargetFilter(Func<IReadOnlyList<IPlayer>> supplier) : this(_ => supplier.Invoke())
        {
        }

        public static implicit operator TargetFilter(Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> filter)
        {
            return new TargetFilter(filter);
        }

        public static TargetFilter Living(IMatch match)
        {
            return new TargetFilter(() => match.LivingPlayers);
        }

        public static TargetFilter Dead(IMatch match)
        {
            return new TargetFilter(() => match.AllPlayers.Where(player => !player.Alive).ToList());
        }

        public static TargetFilter Only(IPlayer player)
        {
            if (player == null) return None();

            return new TargetFilter(() => new List<IPlayer> {player});
        }

        public static TargetFilter None()
        {
            return new TargetFilter(ImmutableList.Create<IPlayer>);
        }

        public static TargetFilter Of(IReadOnlyList<IPlayer> players)
        {
            if (players.Count == 1) return Only(players.First());

            return new TargetFilter(players.ToImmutableList);
        }

        public IReadOnlyList<IPlayer> Filter(IReadOnlyList<IPlayer> players)
        {
            return _filter.Invoke(players);
        }

        public bool Valid(IPlayer target)
        {
            return _filter.Invoke(new List<IPlayer> {target}).Contains(target);
        }

        public TargetFilter Except(IPlayer player)
        {
            return new TargetFilter(players =>
            {
                players = Filter(players);
                return players.Where(entry => entry != player).ToList();
            });
        }

        public TargetFilter Except(ITeam team)
        {
            return new TargetFilter(players =>
            {
                players = Filter(players);
                return players.Where(entry => entry.Role.Team != team).ToList();
            });
        }

        public TargetFilter And(TargetFilter filter)
        {
            return new TargetFilter(players => filter.Filter(Filter(players)));
        }

        public TargetFilter Where(Func<IPlayer, bool> filter)
        {
            return new TargetFilter(players => _filter
                .Invoke(players)
                .Where(filter)
                .ToList());
        }

        public Target Build(IAbility ability, [CanBeNull] TargetNotification message)
        {
            return new Target(ability, this, message);
        }
    }
}