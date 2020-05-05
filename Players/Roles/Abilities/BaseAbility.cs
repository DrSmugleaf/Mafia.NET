using Mafia.NET.Matches;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public abstract class BaseAbility : IAbility
    {
        public IMatch Match { get; }
        public IPlayer User { get; }
        public string Name { get; }
        public AbilityPhase Phase { get; }
        public Targeting Targeting { get; }

        public BaseAbility(IMatch match, IPlayer user, string name, AbilityPhase phase)
        {
            Match = match;
            User = user;
            Name = name;
            Phase = phase;
            Targeting = new Targeting(match);
        }

        protected virtual void OnDayStart() => Expression.Empty();

        protected virtual void OnDayEnd() => Expression.Empty();

        protected virtual void OnNightStart() => Expression.Empty();

        protected virtual void OnNightEnd() => Expression.Empty();
    }
}
