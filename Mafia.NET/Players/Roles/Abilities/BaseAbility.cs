using System;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Town;
using Mafia.NET.Players.Roles.Categories;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; }
        IPlayer User { get; set; }
        string Name { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        TargetManager TargetManager { get; set; }
        IAbilitySetup AbilitySetup { get; set; }
        bool Active { get; set; }
        bool RoleBlockImmune { get; set; }
        bool DeathImmune { get; set; }
        bool CurrentlyDeathImmune { get; set; }
        bool DetectionImmune { get; }
        int Cooldown { get; set; }
        int Uses { get; set; }

        void Initialize(IPlayer user);
        bool TryVictory(out IVictory victory);
        Notification VictoryNotification();
        void AddTarget(TargetFilter filter, TargetNotification message);
        void AddTarget(IPlayer target, TargetNotification message);
        void Try(Action<IPlayer> action);
        void Attack(IPlayer victim);
        void PiercingAttack(IPlayer victim);
        bool HealedBy(IPlayer healer);
        bool BlockedBy(IPlayer blocker);
        bool PiercingBlockedBy(IPlayer blocker);
        bool DetectableBy(ISheriffSetup setup);
        Key Guilty(ISheriffSetup setup);
        bool DetectTarget(out IPlayer target, IIgnoresDetectionImmunity setup = null);
        bool AloneTeam();
        void OnDayStart();
        bool OnDayEnd();
        void OnNightStart();
        void BeforeNightEnd();
        void Chat();
        void Detain();
        void Vest();
        void Switch();
        void Block(IPlayer target);
        void Misc(IPlayer target);
        void Kill(IPlayer target);
        void Protect(IPlayer target);
        void Clean(IPlayer target);
        void Detect(IPlayer target);
        void Disguise(IPlayer target);
        void MasonRecruit(IPlayer target);
        void CultRecruit(IPlayer target);
    }

    public abstract class BaseAbility<T> : IAbility where T : class, IAbilitySetup, new()
    {
        public BaseAbility()
        {
            Active = true;
            Cooldown = 0;
            Uses = 0;
        }

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
        public bool DeathImmune { get; set; }
        public bool CurrentlyDeathImmune { get; set; }
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
            User = user;
            AbilitySetup = Match.AbilitySetups.Setup<T>();

            RoleBlockImmune = Setup is IRoleBlockImmune rbImmuneSetup && rbImmuneSetup.RoleBlockImmune;
            DeathImmune = Setup is INightImmune nImmuneSetup && nImmuneSetup.NightImmune;
            CurrentlyDeathImmune = DeathImmune;
            DetectionImmune = Setup is IDetectionImmune dImmuneSetup && dImmuneSetup.DetectionImmune;

            TargetManager = new TargetManager(Match, User);
        }

        public virtual bool TryVictory(out IVictory victory)
        {
            var living = Match.LivingPlayers;
            var enemies = User.Role.Enemies();
            victory = null;

            if (living.SelectMany(player => player.Role.Goals()).Intersect(enemies).Any()) return false;

            victory = new Victory(User, VictoryNotification());
            return true;
        }

        public virtual Notification VictoryNotification()
        {
            return User.Role.Categories[0].Goal.VictoryNotification(User);
        }

        public void AddTarget(TargetFilter filter, TargetNotification message)
        {
            TargetManager.Add(filter.Build(User, message));
        }

        public void AddTarget(IPlayer target, TargetNotification message)
        {
            AddTarget(TargetFilter.Only(target), message);
        }

        public void Try(Action<IPlayer> action)
        {
            if (TargetManager.Try(out var target)) action(target);
        }

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

        public bool BlockedBy(IPlayer blocker)
        {
            if (RoleBlockImmune) return false;

            Active = false;
            return true;
        }

        public bool PiercingBlockedBy(IPlayer blocker)
        {
            Active = false;
            return true;
        }

        public abstract bool DetectableBy(ISheriffSetup setup);

        public Key Guilty(ISheriffSetup setup)
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
            CurrentlyDeathImmune = DeathImmune;
            TargetManager.Reset();
            Active = true;
            _onDayStart();
        }

        public virtual bool OnDayEnd()
        {
            if (Active && User.Alive) _onDayEnd();
            return Active;
        }

        public virtual void OnNightStart()
        {
            TargetManager.Reset();
            if (Active && User.Alive) _onNightStart();
        }

        public void BeforeNightEnd()
        {
            User.Blackmailed = false;
            Cooldown--;
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

        public virtual void Block(IPlayer target)
        {
        }

        public virtual void Misc(IPlayer target)
        {
        }

        public virtual void Kill(IPlayer target)
        {
        }

        public virtual void Protect(IPlayer target)
        {
        }

        public virtual void Clean(IPlayer target)
        {
        }

        public virtual void Detect(IPlayer target)
        {
        }

        public virtual void Disguise(IPlayer target)
        {
        }

        public virtual void MasonRecruit(IPlayer target)
        {
        }

        public virtual void CultRecruit(IPlayer target)
        {
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