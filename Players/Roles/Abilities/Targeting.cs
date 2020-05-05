using Mafia.NET.Matches;
using Mafia.NET.Players.Teams;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class Targeting
    {
        public IReadOnlyDictionary<TimePhase, PhaseTargeting> Phases { get; }
        public IMatch Match { get; }

        public Targeting(IReadOnlyDictionary<TimePhase, PhaseTargeting> phases, IMatch match)
        {
            Phases = phases;
            Match = match;
        }

        public Targeting(IMatch match)
        {
            var phases = new Dictionary<TimePhase, PhaseTargeting>();

            foreach (TimePhase phase in Enum.GetValues(typeof(TimePhase)))
            {
                phases[phase] = new PhaseTargeting(phase);
            }

            Phases = phases;
            Match = match;
        }

        public PhaseTargeting Get() => Phases[Match.CurrentTime];
    }

    public class PhaseTargeting
    {
        public TimePhase Phase { get; }
        public IList<Target> Targets { get; set; }

        public PhaseTargeting(TimePhase phase, IList<Target> targets)
        {
            Phase = phase;
            Targets = targets;
        }

        public PhaseTargeting(TimePhase phase) : this(phase, new List<Target>())
        {
        }

        public IList<Target> OnStart()
        {
            var old = Targets;
            Targets = new List<Target>();
            return old;
        }
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

        public static implicit operator Target(TargetFilter targeting) => targeting.Make();

        public static TargetFilter Living(IMatch match) => new TargetFilter(() => match.LivingPlayers);

        public static TargetFilter Dead(IMatch match) => new TargetFilter(() => match.Graveyard.Select(death => death.Of).ToDictionary(x => x.Id, x => x));

        public static TargetFilter Only(IPlayer player)
        {
            return new TargetFilter(() =>
            {
                return new Dictionary<int, IPlayer>() { [player.Id] = player };
            });
        }

        public static TargetFilter None() => new TargetFilter(() => ImmutableDictionary.Create<int, IPlayer>());

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

        public Target Make() => new Target(this);
    }
}
