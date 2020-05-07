﻿using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Teams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class TargetManager
    {
        public IReadOnlyDictionary<Time, PhaseTargeting> Phases { get; }
        public IMatch Match { get; }

        public TargetManager(IReadOnlyDictionary<Time, PhaseTargeting> phases, IMatch match)
        {
            Phases = phases;
            Match = match;
        }

        public TargetManager(IMatch match)
        {
            var phases = new Dictionary<Time, PhaseTargeting>();

            foreach (Time phase in Enum.GetValues(typeof(Time)))
            {
                phases[phase] = new PhaseTargeting(phase);
            }

            Phases = phases;
            Match = match;
        }

        public PhaseTargeting Get() => Phases[Match.PhaseManager.CurrentTime];

        public PhaseTargeting Day() => Phases[Time.DAY];

        public PhaseTargeting Night() => Phases[Time.NIGHT];

        public Target this[Time phase, int index]
        {
            get => Phases[phase][index];
            set => Phases[phase][index] = value;
        }

        public Target this[int index]
        {
            get => Get()[index];
            set => Get()[index] = value;
        }

        public void Add(Target target) => Get().Add(target);

        public void Add(params IPlayer[] targets) => Get().Add(targets);

        public void Reset(Target target) => Get().Reset(target);

        public void Reset(params IPlayer[] targets) => Get().Reset(targets);
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

        public Target this[int index]
        {
            get => Targets[index];
            set => Targets[index] = value;
        }

        public void Add(Target target) => Targets.Add(target);

        public void Add(params IPlayer[] targets) => Add(TargetFilter.Of(targets));

        public void Reset(Target target)
        {
            Targets.Clear();
            Targets.Add(target);
        }

        public void Reset(params IPlayer[] targets) => Reset(TargetFilter.Of(targets));
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

        public static implicit operator Target(TargetFilter targeting) => targeting.Build();

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

        public Target Build() => new Target(this);
    }
}