namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class SelfImmunity : AbilityAction
    {
        public SelfImmunity(
            IAbility ability,
            int strength = 1,
            int priority = -3,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
            Strength = strength;
        }

        public SelfImmunity(
            IAbility ability,
            AttackStrength strength = AttackStrength.Base,
            int priority = -3,
            bool direct = true,
            bool stoppable = true) :
            this(ability, (int) strength, priority, direct, stoppable)
        {
        }

        public int Strength { get; set; }

        public override bool Use(IPlayer target)
        {
            if (target != User) return false;

            Ability.CurrentNightImmunity = Strength;
            return true;
        }
    }
}