using System;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public interface IAbilityAction
    {
        IAbility Ability { get; set; }
        IPlayer User { get; }
        IMatch Match { get; }
        TargetManager TargetManager { get; }
        int Priority { get; set; }
        bool Direct { get; set; }
        bool Stoppable { get; set; }
        int Uses { get; }
        int Cooldown { get; }
        Func<IAbilityAction, bool> Filter { get; set; }

        bool TryUse();
    }

    public interface IAbilityAction<T> : IAbilityAction where T : IAbilitySetup
    {
        public T Setup { get; set; }
    }

    public abstract class AbilityAction : IAbilityAction
    {
        protected AbilityAction(
            IAbility ability,
            int priority,
            bool direct = true,
            bool stoppable = true)
        {
            Ability = ability;
            Priority = priority;
            Direct = direct;
            Stoppable = stoppable;
            Filter = action => true;
        }

        public IAbility Ability { get; set; }
        public IPlayer User => Ability.User;
        public IMatch Match => User.Match;
        public TargetManager TargetManager => Ability.TargetManager;
        public int Priority { get; set; }
        public bool Direct { get; set; }
        public bool Stoppable { get; set; }
        public int Uses => Ability.Uses;
        public int Cooldown => Ability.Cooldown;
        public Func<IAbilityAction, bool> Filter { get; set; }

        public virtual bool TryUse()
        {
            if (!Filter(this) || !Ability.Active) return false;
            return ResolveUse();
        }

        public static implicit operator IAbilityAction[](AbilityAction action)
        {
            return new IAbilityAction[] {action};
        }

        public virtual bool ResolveUse()
        {
            TargetManager.Try(out var first);
            TargetManager.Try(1, out var second);

            if (first == null && second == null) return Use();
            if (first != null && second != null) return Use(first, second);
            if (first != null) return Use(first);
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
    }

    public abstract class AbilityAction<T> : AbilityAction, IAbilityAction<T> where T : IAbilitySetup
    {
        protected AbilityAction(
            IAbility<T> ability,
            int priority,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
            Ability = ability;
            Setup = ability.Setup;
        }

        public new IAbility<T> Ability { get; set; }
        public T Setup { get; set; }
    }
}