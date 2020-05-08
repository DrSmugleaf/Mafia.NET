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
        bool Active { get; set; }
        IPlayer Visiting { get; set; }

        bool TryVictory(out IVictory victory);
        void Disable();
        void DisablePiercing();
        void Threaten(IPlayer victim);
        void ThreatenPiercing(IPlayer victim);
        void OnDayStart();
        bool OnDayEnd();
        bool OnNightStart();
        bool OnNightEnd();
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
        public bool Active { get; set; }
        public IPlayer Visiting { get; set; }

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

        public virtual void Disable() => Active = false;

        public void DisablePiercing() => Active = false;

        public virtual void Threaten(IPlayer victim)
        {
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public void ThreatenPiercing(IPlayer victim)
        {
            var threat = new Death(this, victim, true);
            Match.Graveyard.Threats.Add(threat);
        }

        public virtual void OnDayStart()
        {
            TargetManager.Reset();
            Active = true;
            _onDayStart();
        }

        public virtual bool OnDayEnd()
        {
            if (Active) _onDayEnd();
            return Active;
        }

        public virtual bool OnNightStart()
        {
            TargetManager.Reset();

            if (Active) _onNightStart();
            return Active;
        }

        public virtual bool OnNightEnd()
        {
            if (Active) _onNightEnd();
            return Active;
        }

        protected virtual void _onDayStart() => Expression.Empty();

        protected virtual void _onDayEnd() => Expression.Empty();

        protected virtual void _onNightStart() => Expression.Empty();

        protected virtual void _onNightEnd() => Expression.Empty();
    }
}
