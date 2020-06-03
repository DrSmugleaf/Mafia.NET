namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class SerialKillerAttack : AbilityAction<ISerialKillerSetup>
    {
        public SerialKillerAttack(
            IAbility<ISerialKillerSetup> ability,
            int strength = 1,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            base(ability, priority, direct, stoppable)
        {
            Strength = strength;
        }

        public SerialKillerAttack(
            IAbility<ISerialKillerSetup> ability,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            this(ability, (int) strength, direct, stoppable, priority)
        {
        }

        public int Strength { get; set; }

        public override bool TryUse()
        {
            if (!Filter(this) || !Ability.Active && !Setup.KillsRoleBlockers) return false;
            return ResolveUse();
        }

        public override bool Use(IPlayer target)
        {
            var attack = new Attack(Ability, Strength, Direct, Stoppable, Priority);
            return attack.Use(target);
        }
    }

    public interface ISerialKillerSetup : INightImmune, IDetectionImmune
    {
        public bool KillsRoleBlockers { get; set; }
        public bool WinsTiesOverArsonist { get; set; }
    }
}