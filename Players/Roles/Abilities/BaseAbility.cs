using Mafia.NET.Matches;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public abstract class BaseAbility<T> : IAbility<T> where T : ITarget
    {
        public IMatch Match { get; }
        public IPlayer User { get; }
        public string Name { get; }
        public AbilityPhase Phase { get; }
        public T Target { get; protected set; }

        public BaseAbility(IMatch match, IPlayer user, string name, AbilityPhase phase)
        {
            Match = match;
            User = user;
            Name = name;
            Phase = phase;
        }

        public virtual bool UsableDay(T target)
        {
            return Phase == AbilityPhase.DAY || Phase == AbilityPhase.BOTH && CanTarget(target);
        }

        public virtual bool UsableNight(T target)
        {
            return Phase == AbilityPhase.NIGHT || Phase == AbilityPhase.BOTH && CanTarget(target);
        }

        public bool TryUse(T target)
        {
            if (Match.CurrentTime == TimePhase.DAY && UsableDay(target))
            {
                UseDay(target);
                return true;
            } else if (Match.CurrentTime == TimePhase.NIGHT && UsableNight(target))
            {
                UseNight(target);
                return true;
            }

            return false;
        }

        public virtual void UseDay(T target) => Expression.Empty();

        public virtual void UseNight(T target) => Expression.Empty();

        public virtual bool CanTarget(IPlayer target)
        {
            return target != User;
        }

        public bool CanTarget(T target)
        {
            return ValidTargets().Any(valid => target.Targets(valid.Value));
        }

        public IDictionary<int, IPlayer> ValidTargets()
        {
            return Match.LivingPlayers.Where(living => CanTarget(living.Value)).ToDictionary(x => x.Key, x => x.Value);
        }

        public virtual void OnDayStart() => Expression.Empty();

        public virtual void OnDayEnd() => Expression.Empty();

        public virtual void OnNightStart() => Expression.Empty();

        public virtual void OnNightEnd() => Expression.Empty();
    }
}
