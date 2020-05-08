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
        bool CurrentlyDeathImmune { get; set; }
        int Cooldown { get; set; }

        bool TryVictory(out IVictory victory);
        Notification VictoryNotification();
        void AddTarget(TargetFilter filter, TargetMessage message);
        void AddTarget(IPlayer target, TargetMessage message);
        void Disable();
        void DisablePiercing();
        void Threaten(IPlayer victim);
        void ThreatenPiercing(IPlayer victim);
        bool AloneTeam();
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
        protected bool DeathImmunity { get; }
        public bool CurrentlyDeathImmune { get; set; }
        private int _cooldown { get; set; }
        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = _cooldown > 0 ? --_cooldown : 0;
        }

        public BaseAbility()
        {
            AbilitySetup = (T)Match.Setup.Roles.Abilities[Name];
            DeathImmunity = false;
            CurrentlyDeathImmune = DeathImmunity;
            Cooldown = 0;
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

        public void AddTarget(TargetFilter filter, TargetMessage message)
        {
            TargetManager.Add(filter.Build(User, message));
        }

        public void AddTarget(IPlayer target, TargetMessage message)
        {
            AddTarget(TargetFilter.Only(target), message);
        }

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

        public bool AloneTeam()
        {
            return Match.LivingPlayers.Values.Where(player => player.Role.Affiliation == User.Role.Affiliation).Count() == 1;
        }

        public virtual void OnDayStart()
        {
            CurrentlyDeathImmune = DeathImmunity;
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
            if (Active)
            {
                if (Setup is ICooldownSetup cooldownSetup)
                {
                    if (Cooldown > 0)
                    {
                        Cooldown--;
                        return false;
                    }
                    else
                    {
                        var action = _onNightEnd();
                        if (action) Cooldown = cooldownSetup.Cooldown;
                        return action;
                    }
                }
                else
                {
                    return _onNightEnd();
                }
            }
            else
            {
                Cooldown--;
                return false;
            }
        }

        protected virtual void _onDayStart() => Expression.Empty();

        protected virtual void _onDayEnd() => Expression.Empty();

        protected virtual void _onNightStart() => Expression.Empty();

        protected virtual bool _onNightEnd() => false;
    }

    public interface ICooldownSetup
    {
        int Cooldown { get; set; }
    }
}
