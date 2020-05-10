using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Teams;

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

            foreach (Time phase in Enum.GetValues(typeof(Time))) phases[phase] = new PhaseTargeting(phase);

            Phases = phases;
        }
#nullable enable
        public PhaseTargeting Get()
        {
            return Phases[Match.Phase.CurrentTime];
        }

        public PhaseTargeting Day()
        {
            return Phases[Time.DAY];
        }

        public IPlayer? Day(int index)
        {
            return this[Time.DAY, index];
        }

        public PhaseTargeting Night()
        {
            return Phases[Time.NIGHT];
        }

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

        public bool Try(int index, out IPlayer? target)
        {
            return Try(Match.Phase.CurrentTime, index, out target);
        }

        public bool Try(out IPlayer? target)
        {
            return Try(0, out target);
        }

        public bool TryDay(int index, out IPlayer? target)
        {
            return Try(Time.DAY, index, out target);
        }

        public bool TryNight(int index, out IPlayer? target)
        {
            return Try(Time.NIGHT, index, out target);
        }

        public void Add(Target target)
        {
            Get().Add(target);
        }

        public void Add(TargetNotification message, params IPlayer[] targets)
        {
            Get().Add(User, message, targets);
        }

        public void Set(IPlayer? target)
        {
            Get().Set(User, target);
        }

        public void ForceSet(IPlayer? target)
        {
            Get().ForceSet(User, target);
        }

        public void Reset()
        {
            Get().Reset();
        }

        public void Reset(Target target)
        {
            Get().Reset(target);
        }

        public void Reset(TargetNotification message, params IPlayer[] targets)
        {
            Get().Reset(User, message, targets);
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

        public Target? this[int index]
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

        public void Add(IPlayer user, TargetNotification? message = null, params IPlayer[] targets)
        {
            Add(TargetFilter.Of(targets).Build(user, message));
        }

        public void Set(IPlayer user, IPlayer? target)
        {
            if (Targets.Count == 0) Add(new Target(user, TargetFilter.Any));
            Targets[0].Targeted = target;
        }

        public void ForceSet(IPlayer user, IPlayer? target)
        {
            if (Targets.Count == 0) Add(new Target(user, TargetFilter.Any));
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

        public void Reset(IPlayer user, TargetNotification message, params IPlayer[] targets)
        {
            Reset(TargetFilter.Of(targets).Build(user, message));
        }
    }

    public class TargetFilter
    {
        public static readonly TargetFilter Any = new TargetFilter(dictionary => dictionary);

        private TargetFilter(Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> filter)
        {
            _filter = filter;
        }

        private TargetFilter(Func<IReadOnlyList<IPlayer>> supplier) : this(_ => supplier.Invoke())
        {
        }

        private Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> _filter { get; }

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

            return new TargetFilter(() => new List<IPlayer> {[player.Id] = player});
        }

        public static TargetFilter None()
        {
            return new TargetFilter(() => ImmutableList.Create<IPlayer>());
        }

        public static TargetFilter Of(IEnumerable<IPlayer> players)
        {
            if (players.Count() == 1) return Only(players.First());

            return new TargetFilter(players.ToImmutableList);
        }

        public IReadOnlyList<IPlayer> Filter(IReadOnlyList<IPlayer> players)
        {
            return _filter.Invoke(players);
        }

        public bool Valid(IPlayer target)
        {
            return _filter.Invoke(new List<IPlayer> {[target.Id] = target}).Count > 0;
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
            return new TargetFilter(dictionary =>
            {
                dictionary = Filter(dictionary);
                return dictionary.Where(entry => entry.Role.Affiliation != team).ToList();
            });
        }

        public TargetFilter And(TargetFilter filter)
        {
            return new TargetFilter(dictionary => filter.Filter(Filter(dictionary)));
        }

        public Target Build(IPlayer user, TargetNotification? message)
        {
            return new Target(user, this, message);
        }
    }
}