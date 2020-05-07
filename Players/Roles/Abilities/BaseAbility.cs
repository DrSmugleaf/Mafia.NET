using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Categories;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public abstract class BaseAbility<T> : IAbility where T : IAbilitySetup
    {
        public IMatch Match { get; set; }
        public IPlayer User { get; set; }
        public string Name { get; set; }
        public Targeting Targeting { get; set; }
        public MessageRandomizer MurderDescriptions { get; set; }
        public IAbilitySetup AbilitySetup { get; }
        public T Setup { get => (T)AbilitySetup; }

        public BaseAbility()
        {
            AbilitySetup = (T)Match.Setup.Roles.Abilities[Name];
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
