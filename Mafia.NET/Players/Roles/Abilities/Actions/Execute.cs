namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Execute : AbilityAction
    {
        public Execute(
            IAbility ability,
            int strength = 1,
            int priority = 5,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
            Strength = strength;
            Filter = action =>
                action.TargetManager.Try(out var target) &&
                action.Uses > 0 &&
                action.TargetManager.TryDay(out var prisoner) &&
                target == prisoner;
        }

        public Execute(
            IAbility ability,
            AttackStrength strength = AttackStrength.Base,
            int priority = 5,
            bool direct = true,
            bool stoppable = true) :
            this(ability, (int) strength, priority, direct, stoppable)
        {
        }

        public int Strength { get; set; }

        public override bool Use(IPlayer target)
        {
            var attack = new Attack(Ability, Strength, Direct, Stoppable, Priority);
            var vulnerable = attack.VictimVulnerable(target);
            attack.Use(target);

            Ability.Uses--;

            return vulnerable;
        }
    }
}