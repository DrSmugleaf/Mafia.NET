using System;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;
using Mafia.NET.Players.Roles.Categories;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility : IAbilityAction
    {
        IMatch Match { get; }
        IPlayer User { get; set; }
        string Name { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        TargetManager TargetManager { get; set; }
        IAbilitySetup AbilitySetup { get; set; }
        bool Active { get; set; }
        bool RoleBlockImmune { get; set; }
        bool NightImmune { get; set; }
        bool CurrentlyNightImmune { get; set; }
        bool DetectionImmune { get; set; }
        int Cooldown { get; set; }
        int Uses { get; set; }

        void Initialize(IPlayer user);
        bool TryVictory(out IVictory victory);
        Notification VictoryNotification();
        void AddTarget(TargetFilter filter, TargetNotification message);
        void AddTarget(IPlayer target, TargetNotification message);
        bool Attack(IPlayer victim);
        void PiercingAttack(IPlayer victim);
        bool HealedBy(IPlayer healer);
        bool BlockedBy(IPlayer blocker);
        bool PiercingBlockedBy(IPlayer blocker);
        bool DetectableBy(ISheriffSetup setup);
        Key DirectSheriff(ISheriffSetup setup);
        bool DetectTarget(out IPlayer target, IIgnoresDetectionImmunity setup = null);
        bool AloneTeam();
        void OnDayStart();
        bool OnDayEnd();
        void OnNightStart();
        void BeforeNightEnd();
        void OnNightEnd();
    }

    public abstract class BaseAbility<T> : IAbility where T : class, IAbilitySetup, new()
    {
        public T Setup => (T) AbilitySetup;
        private int _cooldown { get; set; }
        private int _uses { get; set; }
        public IMatch Match => User.Match;
        public IPlayer User { get; set; }
        public string Name { get; set; }
        public MessageRandomizer MurderDescriptions { get; set; }
        public TargetManager TargetManager { get; set; }
        public IAbilitySetup AbilitySetup { get; set; }
        public bool Active { get; set; }
        public bool RoleBlockImmune { get; set; }
        public bool NightImmune { get; set; }
        public bool CurrentlyNightImmune { get; set; }
        public bool DetectionImmune { get; set; }

        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = value >= 0 ? value : 0;
        }

        public int Uses
        {
            get => _uses;
            set => _uses = value >= 0 ? value : 0;
        }

        public virtual void Initialize(IPlayer user)
        {
            InitializeBase(user);
        }

        public virtual bool TryVictory(out IVictory victory)
        {
            var living = Match.LivingPlayers;
            var enemies = User.Role.Enemies();
            victory = null;

            if (living.SelectMany(player => player.Role.Goals()).Intersect(enemies).Any())
                return false;

            victory = new Victory(User, VictoryNotification());
            return true;
        }

        public virtual Notification VictoryNotification()
        {
            return User.Role.Categories[0].Goal.VictoryNotification(User);
        }

        public void AddTarget(TargetFilter filter, TargetNotification message)
        {
            TargetManager.Add(filter.Build(this, message));
        }

        public void AddTarget(IPlayer target, TargetNotification message)
        {
            AddTarget(TargetFilter.Only(target), message);
        }

        public virtual void Try(Action<IAbilityAction> action)
        {
            if (Active) action(this);
        }

        public virtual bool Attack(IPlayer victim)
        {
            if (victim.Role.Ability.CurrentlyNightImmune) return false;

            var threat = new Death(this, victim);
            User.Crimes.Add(CrimeKey.Murder);
            Match.Graveyard.Threats.Add(threat);
            return true;
        }

        public void PiercingAttack(IPlayer victim)
        {
            var threat = new Death(this, victim);
            User.Crimes.Add(CrimeKey.Murder);
            Match.Graveyard.Threats.Add(threat);
        }

        public virtual bool HealedBy(IPlayer healer)
        {
            var threats = Match.Graveyard.ThreatsOn(User);
            if (threats.Count > 0)
            {
                var threat = threats[0];
                Match.Graveyard.Threats.Remove(threat);

                return true;
            }

            return false;
        }

        public virtual bool BlockedBy(IPlayer blocker)
        {
            if (RoleBlockImmune) return false;

            Active = false;
            return true;
        }

        public virtual bool PiercingBlockedBy(IPlayer blocker)
        {
            Active = false;
            return true;
        }

        public abstract bool DetectableBy(ISheriffSetup setup);

        public Key DirectSheriff(ISheriffSetup setup)
        {
            return !DetectableBy(setup) || DetectionImmune ? SheriffKey.NotSuspicious : GuiltyName();
        }

        public bool DetectTarget(out IPlayer target, IIgnoresDetectionImmunity setup = null)
        {
            target = null;

            var ignoresImmunity = setup?.IgnoresDetectionImmunity ?? false;
            if (ignoresImmunity || !DetectionImmune) target = TargetManager[0];

            return target != null;
        }

        public bool AloneTeam()
        {
            return Match.LivingPlayers
                .Count(player => player.Role.Team == User.Role.Team) == 1;
        }

        public virtual void OnDayStart()
        {
            User.Crimes.Framing = null;
            CurrentlyNightImmune = NightImmune;
            Active = true;
            _onDayStart();
        }

        public virtual bool OnDayEnd()
        {
            TargetManager.Reset(Time.Night);
            if (Active && User.Alive) _onDayEnd();
            return Active;
        }

        public virtual void OnNightStart()
        {
            if (Active && User.Alive) _onNightStart();
        }

        public void BeforeNightEnd()
        {
            User.Blackmailed = false;
            Cooldown--;
        }

        public void OnNightEnd()
        {
            TargetManager.Reset(Time.Day);
        }

        public virtual void Chat()
        {
        }

        public virtual void Detain()
        {
        }

        public virtual void Vest()
        {
        }

        public virtual void Switch()
        {
        }

        public virtual void Block()
        {
        }

        public virtual void Misc()
        {
        }

        public virtual void Kill()
        {
        }

        public virtual void Protect()
        {
        }

        public virtual void Clean()
        {
        }

        public virtual void Detect()
        {
        }

        public virtual void Disguise()
        {
        }

        public virtual void MasonRecruit()
        {
        }

        public virtual void CultRecruit()
        {
        }

        public virtual void Revenge()
        {
        }

        public void InitializeBase(IPlayer user)
        {
            Active = true;
            User = user;
            AbilitySetup = Match.AbilitySetups.Setup<T>();

            RoleBlockImmune = Setup is IRoleBlockImmune rbImmuneSetup && rbImmuneSetup.RoleBlockImmune;
            NightImmune = Setup is INightImmune nImmuneSetup && nImmuneSetup.NightImmune;
            CurrentlyNightImmune = NightImmune;
            DetectionImmune = Setup is IDetectionImmune dImmuneSetup && dImmuneSetup.DetectionImmune;
            Cooldown = Setup is ICooldownSetup cooldownSetup ? cooldownSetup.NightsBetweenUses : 0;
            Uses = Setup is IUsesSetup chargeSetup ? chargeSetup.Uses : 0;

            TargetManager = new TargetManager(Match, this);
        }

        protected abstract Key GuiltyName();

        protected virtual void _onDayStart()
        {
        }

        protected virtual void _onDayEnd()
        {
        }

        protected virtual void _onNightStart()
        {
        }
    }
}