using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities.Bases
{
    public interface IAbility
    {
        IPlayer User { get; set; }
        IAbilitySetup Setup { get; set; }
        IRole Role { get; }
        IMatch Match { get; }
        TargetManager Targets { get; }
        int Priority { get; set; }
        bool RoleBlocked { get; }
        int Uses { get; }
        int Cooldown { get; }
        Func<IAbility, bool> Filter { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }

        void Initialize(AbilityEntry entry, IPlayer user);
        void FromParent(IAbility ability);
        bool Is(Type type);
        void DayStart(in IList<IAbility> abilities);
        void DayEnd(in IList<IAbility> abilities);
        void NightStart(in IList<IAbility> abilities);
        void NightEnd(in IList<IAbility> abilities);
        bool ResolveUse();
    }

    public interface IAbility<T> : IAbility where T : IAbilitySetup
    {
        public new T Setup { get; set; }
    }

    public abstract class Ability : IAbility
    {
        private int _cooldown;
        private int _uses;

        protected Ability()
        {
            Filter = ability => true;
        }

        public bool Initialized { get; private set; }

        public IPlayer User { get; set; }
        public IAbilitySetup Setup { get; set; }
        public IRole Role => User.Role;
        public IMatch Match => User.Match;
        public TargetManager Targets => User.Targets;
        public int Priority { get; set; }
        public bool RoleBlocked => User.Perks.RoleBlocked;

        public int Uses
        {
            get => _uses;
            set => _uses = value >= 0 ? value : 0;
        }

        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = value >= 0 ? value : 0;
        }

        public Func<IAbility, bool> Filter { get; set; }
        public MessageRandomizer MurderDescriptions { get; set; }

        public virtual void Initialize(AbilityEntry entry, IPlayer user)
        {
            if (Initialized) return;

            User = user;
            Setup = user.Match.AbilitySetups.Setup(entry);
            MurderDescriptions = entry.MurderDescriptions;
            Uses = Setup is IUsesSetup usesSetup ? usesSetup.Uses : 0;
            Priority = entry.Priority;

            Initialized = true;
        }

        public virtual void FromParent(IAbility ability)
        {
            User = ability.User;
            Setup = ability.Setup;
            MurderDescriptions = ability.MurderDescriptions;
            Uses = ability.Uses;
        }

        public bool Is(Type type)
        {
            return GetType() == type || type.IsSubclassOf(GetType());
        }

        public virtual void DayStart(in IList<IAbility> abilities)
        {
        }

        public virtual void DayEnd(in IList<IAbility> abilities)
        {
        }

        public virtual void NightStart(in IList<IAbility> abilities)
        {
        }

        public virtual void NightEnd(in IList<IAbility> abilities)
        {
        }

        public virtual bool ResolveUse()
        {
            var targets = ResolveTargets();
            var first = targets[0]?.Targeted;
            var second = targets[1]?.Targeted;

            if (first == null && second == null) return TryUse();
            if (first != null && second != null) return TryUse(first, second);
            if (first != null) return TryUse(first);
            return false;
        }

        public virtual PhaseTargeting ResolveTargets()
        {
            return Targets.GetAll();
        }

        public virtual bool Active()
        {
            return !RoleBlocked;
        }

        public virtual bool CanUseAny()
        {
            return Filter(this) && Active();
        }

        public virtual bool CanUse()
        {
            return CanUseAny();
        }

        public virtual bool CanUse(IPlayer target)
        {
            return CanUseAny();
        }

        public virtual bool CanUse(IPlayer first, IPlayer second)
        {
            return CanUseAny();
        }

        public virtual bool TryUse()
        {
            if (CanUse()) return Use();
            return false;
        }

        public virtual bool TryUse(IPlayer target)
        {
            if (CanUse(target)) return Use(target);
            return false;
        }

        public virtual bool TryUse(IPlayer first, IPlayer second)
        {
            if (CanUse(first, second)) return Use(first, second);
            return false;
        }

        public virtual bool Use()
        {
            return false;
        }

        public virtual bool Use(IPlayer target)
        {
            return false;
        }

        public virtual bool Use(IPlayer first, IPlayer second)
        {
            return false;
        }

        [Pure]
        public T Get<T>() where T : IAbility, new()
        {
            var ability = Match.Abilities.Ability<T>(User);
            ability.FromParent(this);
            return ability;
        }

        [Pure]
        public Attack Attack(
            AttackStrength strength = AttackStrength.Base,
            int? priority = null,
            bool direct = true,
            bool stoppable = true)
        {
            var attack = new Attack
            {
                User = User,
                Strength = strength,
                Priority = priority ?? 5,
                Direct = direct,
                Stoppable = stoppable,
                Setup = Setup,
                MurderDescriptions = MurderDescriptions
            };

            return attack;
        }

        [Pure]
        public SetupTargets SetupTargets(TargetFilter filter, TargetNotification notification)
        {
            var targets = new SetupTargets(filter, notification) {User = User};
            return targets;
        }

        [Pure]
        public SetupTargets SetupTargets(IPlayer target, TargetNotification notification)
        {
            return SetupTargets(TargetFilter.Only(target), notification);
        }

        public void SetupTargets(in IList<IAbility> abilities, TargetFilter filter, TargetNotification notification)
        {
            var targets = SetupTargets(filter, notification);
            abilities.Add(targets);
        }

        public void SetupTargets<T>(in IList<IAbility> abilities, TargetFilter filter) where T : Enum
        {
            var notification = TargetNotification.Enum<T>(this);
            SetupTargets(abilities, filter, notification);
        }

        public void SetupTargets(in IList<IAbility> abilities, IPlayer target, TargetNotification notification)
        {
            var targets = SetupTargets(target, notification);
            abilities.Add(targets);
        }

        public void SetupTargets<T>(in IList<IAbility> abilities, IPlayer target) where T : Enum
        {
            var notification = TargetNotification.Enum<T>(this);
            SetupTargets(abilities, target, notification);
        }
    }

    public abstract class Ability<T> : Ability, IAbility<T> where T : IAbilitySetup
    {
        public new T Setup { get; set; }

        public override void Initialize(AbilityEntry entry, IPlayer player)
        {
            if (Initialized) return;

            base.Initialize(entry, player);
            Setup = (T) base.Setup;
        }

        public override void FromParent(IAbility ability)
        {
            base.FromParent(ability);
            Setup = (T) ability.Setup;
        }
    }
}