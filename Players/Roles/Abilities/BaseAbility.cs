using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Categories;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; set; }
        IPlayer User { get; set; }
        string Name { get; set; }
        TargetManager TargetManager { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        IAbilitySetup AbilitySetup { get; }

        bool TryVictory(out IVictory victory);
        void OnDayStart();
        void OnDayEnd();
        void OnNightStart();
        void OnNightEnd();
    }

    public abstract class BaseAbility<T> : IAbility where T : IAbilitySetup
    {
        public IMatch Match { get; set; }
        public IPlayer User { get; set; }
        public string Name { get; set; }
        public TargetManager TargetManager { get; set; }
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

        public void Threaten(IPlayer victim)
        {
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public void OnDayStart()
        {
            TargetManager.Reset();
            _onDayStart();
        }

        public void OnDayEnd() => _onDayEnd();

        public void OnNightStart()
        {
            TargetManager.Reset();
            _onNightStart();
        }

        public void OnNightEnd() => _onNightEnd();

        protected virtual void _onDayStart() => Expression.Empty();

        protected virtual void _onDayEnd() => Expression.Empty();

        protected virtual void _onNightStart() => Expression.Empty();

        protected virtual void _onNightEnd() => Expression.Empty();
    }
}
