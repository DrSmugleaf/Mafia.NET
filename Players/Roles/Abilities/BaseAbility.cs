using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Categories;
using System.Linq;
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
        public MessageRandomizer MurderDescriptions { get; set; }

        public BaseAbility(AbilityEntry entry, IMatch match, IPlayer user, AbilityPhase phase)
        {
            Match = match;
            User = user;
            Name = entry.Name;
            Phase = phase;
            Targeting = new Targeting(match);
            MurderDescriptions = entry.MurderDescriptions;
        }

        public virtual bool TryVictory(out IVictory victory)
        {
            var living = Match.LivingPlayers.Values;
            var enemies = User.Role.Enemies();
            victory = null;

            if (living.SelectMany(player => player.Role.Goals()).Intersect(enemies).Any()) return false;

            victory = new Victory(User, VictoryNotification());
            return true;
        }

        public virtual Notification VictoryNotification() => User.Role.Categories[0].Goal.VictoryNotification(User);

        protected virtual void OnDayStart() => Expression.Empty();

        protected virtual void OnDayEnd() => Expression.Empty();

        protected virtual void OnNightStart() => Expression.Empty();

        protected virtual void OnNightEnd() => Expression.Empty();
    }
}
