using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Teams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetManager
    {
        public IMatch Match { get; }
        public IPlayer User { get; }
        public IReadOnlyDictionary<Time, PhaseTargeting> Phases { get; }

        public TargetManager(IMatch match, IPlayer user, IReadOnlyDictionary<Time, PhaseTargeting> phases)
        {
            Match = match;
            User = user;
            Phases = phases;
        }
#nullable disable
        public TargetManager(IMatch match, IPlayer user)
        {
            Match = match;
            User = user;

            var phases = new Dictionary<Time, PhaseTargeting>();

            foreach (Time phase in Enum.GetValues(typeof(Time)))
            {
                phases[phase] = new PhaseTargeting(phase);
            }

            Phases = phases;
        }
#nullable enable
        public PhaseTargeting Get() => Phases[Match.PhaseManager.CurrentTime];

        public PhaseTargeting Day() => Phases[Time.DAY];

        public IPlayer? Day(int index)
        {
            return this[Time.DAY, index];
        }

        public PhaseTargeting Night() => Phases[Time.NIGHT];

        public IPlayer? Night(int index)
        {
            return this[Time.NIGHT, index];
        }

        public IPlayer? this[Time phase, int index]
        {
            get => Phases[phase][index]?.Targeted;
            set => Phases[phase][index]?.Set(value);
        }

        public IPlayer? this[int index]
        {
            get => Get()[index]?.Targeted;
            set => Get()[index]?.Set(value);
        }

        public bool Try(Time phase, int index, out IPlayer? target)
        {
            target = this[phase, index];
            return target != null;
        }

        public bool Try(int index, out IPlayer? target) => Try(Match.PhaseManager.CurrentTime, index, out target);

        public bool TryDay(int index, out IPlayer? target) => Try(Time.DAY, index, out target);

        public bool TryNight(int index, out IPlayer? target) => Try(Time.NIGHT, index, out target);

        public void Add(Target target) => Get().Add(target);

        public void Add(TargetMessage message, params IPlayer[] targets) => Get().Add(User, message, targets);

        public void Reset() => Get().Reset();

        public void Reset(Target target) => Get().Reset(target);

        public void Reset(TargetMessage message, params IPlayer[] targets) => Get().Reset(User, message, targets);
    }

    public class PhaseTargeting
    {
        public Time Phase { get; }
        public IList<Target> Targets { get; set; }

        public PhaseTargeting(Time phase, IList<Target> targets)
        {
            Phase = phase;
            Targets = targets;
        }

        public PhaseTargeting(Time phase) : this(phase, new List<Target>())
        {
        }

        public Target? this[int index]
        {
            get => index < Targets.Count ? Targets[index] : null;
            set
            {
                if (value == null) return;
                Targets[index] = value;
            }
        }

        public void Add(Target target) => Targets.Add(target);

        public void Add(IPlayer user, TargetMessage message, params IPlayer[] targets) => Add(TargetFilter.Of(targets).Build(user, message));

        public void Reset() => Targets.Clear();

        public void Reset(Target target)
        {
            Reset();
            Targets.Add(target);
        }

        public void Reset(IPlayer user, TargetMessage message, params IPlayer[] targets) => Reset(TargetFilter.Of(targets).Build(user, message));
    }

    public class TargetFilter
    {
        private Func<IReadOnlyDictionary<int, IPlayer>, IReadOnlyDictionary<int, IPlayer>> _filter { get; }

        private TargetFilter(Func<IReadOnlyDictionary<int, IPlayer>, IReadOnlyDictionary<int, IPlayer>> filter)
        {
            _filter = filter;
        }

        private TargetFilter(Func<IReadOnlyDictionary<int, IPlayer>> supplier) : this(_ => supplier.Invoke())
        {
        }

        public static TargetFilter Living(IMatch match) => new TargetFilter(() => match.LivingPlayers);

        public static TargetFilter Dead(IMatch match) => new TargetFilter(() => match.AllPlayers.Where(player => !player.Value.Alive).ToDictionary(x => x.Key, x => x.Value));

        public static TargetFilter Only(IPlayer player)
        {
            if (player == null) return None();

            return new TargetFilter(() =>
            {
                return new Dictionary<int, IPlayer>() { [player.Id] = player };
            });
        }

        public static TargetFilter None() => new TargetFilter(() => ImmutableDictionary.Create<int, IPlayer>());

        public static TargetFilter Of(IEnumerable<IPlayer> players)
        {
            if (players.Count() == 1) return Only(players.First());

            return new TargetFilter(() => players.ToImmutableDictionary(x => x.Id, x => x));
        }

        public IReadOnlyDictionary<int, IPlayer> Filter(IReadOnlyDictionary<int, IPlayer> players)
        {
            return _filter.Invoke(players);
        }

        public TargetFilter Except(IPlayer player)
        {
            return new TargetFilter(players =>
            {
                players = Filter(players);
                return players.Where(entry => entry.Value != player).ToDictionary(x => x.Key, x => x.Value);
            });
        }

        public TargetFilter Except(ITeam team)
        {
            return new TargetFilter(dictionary =>
            {
                dictionary = Filter(dictionary);
                return dictionary.Where(entry => entry.Value.Role.Affiliation != team).ToDictionary(x => x.Key, x => x.Value);
            });
        }

        public TargetFilter And(TargetFilter filter)
        {
            return new TargetFilter(dictionary => filter.Filter(Filter(dictionary)));
        }

        public Target Build(IPlayer user, TargetMessage message) => new Target(user, this, message);
    }
}
