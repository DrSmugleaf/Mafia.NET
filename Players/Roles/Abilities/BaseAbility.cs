using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Categories;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; set; }
        IPlayer User { get; set; }
        string Name { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        TargetManager TargetManager { get; set; }
        IAbilitySetup AbilitySetup { get; set; }
        bool Active { get; set; }
        bool RoleBlockImmune { get; }
        bool DeathImmune { get; }
        bool CurrentlyDeathImmune { get; set; }
        bool DetectionImmune { get; }
        int Cooldown { get; set; }
        int Uses { get; set; }

        void Initialize();
        bool TryVictory(out IVictory victory);
        Notification VictoryNotification();
        void AddTarget(TargetFilter filter, TargetMessage message);
        void AddTarget(IPlayer target, TargetMessage message);
        void Try(Action<IPlayer> action);
        void Disable();
        void PiercingDisable();
        void Attack(IPlayer victim);
        void PiercingAttack(IPlayer victim);
        bool DetectableBy(ISheriffSetup setup);
        string Guilty(ISheriffSetup setup);
        bool AloneTeam();
        void OnDayStart();
        bool OnDayEnd();
        void OnNightStart(); // Detainments, chats
        void BeforeNightEnd();
    }

    public abstract class BaseAbility<T> : IAbility where T : IAbilitySetup, new()
    {
        public IMatch Match { get; set; }
        public IPlayer User { get; set; }
        public string Name { get; set; }
        public MessageRandomizer MurderDescriptions { get; set; }
        public TargetManager TargetManager { get; set; }
        public IAbilitySetup AbilitySetup { get; set; }
        public T Setup { get => (T)AbilitySetup; }
        public bool Active { get; set; }
        public bool RoleBlockImmune { get; }
        public bool DeathImmune { get; }
        public bool CurrentlyDeathImmune { get; set; }
        public bool DetectionImmune { get; }
        private int _cooldown { get; set; }
        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = value >= 0 ? value : 0;
        }
        private int _uses { get; set; }
        public int Uses
        {
            get => _uses;
            set => _uses = value >= 0 ? value : 0;
        }

        public BaseAbility()
        {
            Active = true;
            RoleBlockImmune = Setup is IRoleBlockImmune rbImmuneSetup ? rbImmuneSetup.RoleBlockImmune : false;
            DeathImmune = false;
            CurrentlyDeathImmune = DeathImmune;
            Cooldown = 0;
            Uses = 0;
        }

        public virtual void Initialize()
        {
            AbilitySetup = Match.Abilities.Setup<T>();
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

        public void Try(Action<IPlayer> action)
        {
            if (TargetManager.Try(out var target)) action(target);
        }

        public virtual void Disable()
        {
            if (RoleBlockImmune) return;
            Active = false;
        }

        public void PiercingDisable() => Active = false;

        public virtual void Attack(IPlayer victim)
        {
            if (victim.Role.Ability.CurrentlyDeathImmune) return;
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public void PiercingAttack(IPlayer victim)
        {
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public abstract bool DetectableBy(ISheriffSetup setup);

        public string Guilty(ISheriffSetup setup)
        {
            return !DetectableBy(setup) || DetectionImmune ? "Not Suspicious" : GuiltyName();
        }

        protected abstract string GuiltyName();

        public bool AloneTeam()
        {
            return Match.LivingPlayers.Values
                .Where(player => player.Role.Affiliation == User.Role.Affiliation)
                .Count() == 1;
        }

        public virtual void OnDayStart()
        {
            User.Crimes.Framing = null;
            CurrentlyDeathImmune = DeathImmune;
            TargetManager.Reset();
            Active = true;
            _onDayStart();
        }

        public virtual bool OnDayEnd()
        {
            if (Active) _onDayEnd();
            return Active;
        }

        public virtual void OnNightStart()
        {
            TargetManager.Reset();
            if (Active) _onNightStart();
        }

        public void BeforeNightEnd()
        {
            User.Blackmailed = false;
            Cooldown--;
        }

        protected virtual void _onDayStart() => Expression.Empty();

        protected virtual void _onDayEnd() => Expression.Empty();

        protected virtual void _onNightStart() => Expression.Empty();
    }

    public interface ICooldownSetup
    {
        int NightsBetweenUses { get; set; }
    }

    public interface IChargeSetup
    {
        int Charges { get; set; }
    }

    public interface INightImmune
    {
        bool NightImmune { get; set; }
    }

    public interface IRoleBlockImmune
    {
        bool RoleBlockImmune { get; set; }
    }

    public interface IDetectionImmune
    {
        bool DetectionImmune { get; set; }
    }

    public interface ISheriffSetup
    {
        bool DetectsMafiaTriad { get; set; }
        bool DetectsSerialKiller { get; set; }
        bool DetectsArsonist { get; set; }
        bool DetectsCult { get; set; }
        bool DetectsMassMurderer { get; set; }
    }

    public interface IRandomExcluded
    {
        bool ExcludedFromRandoms { get; set; }
    }

    public interface INightChatter : IAbility
    {
        public void Chat();
    }

    public interface IDetainer : IAbility
    {
        public void Detain(IPlayer prisoner);
    }

    public interface IVest : IAbility
    {
        public void Vest();
    }

    public interface ISwitcher : IAbility
    {
        public void Switch();
    }
    
    public interface IRoleBlocker : IAbility
    {
        public void Block(IPlayer target);
    }

    public interface IMisc : IAbility
    {
        public void Misc(IPlayer target);
    }

    public interface IKiller : IAbility
    {
        public void Kill(IPlayer target);
    }

    public interface ICleaner : IAbility
    {
        public void Clean(IPlayer target);
    }

    public interface IDetector : IAbility
    {
        public void Detect(IPlayer target);
    }

    public interface IDisguiser : IAbility
    {
        public void Disguise(IPlayer target);
    }

    public interface IMasonRecruiter : IAbility
    {
        public void MasonRecruit(IPlayer target);
    }

    public interface ICultRecruiter : IAbility
    {
        public void CultRecruit(IPlayer target);
    }
}
