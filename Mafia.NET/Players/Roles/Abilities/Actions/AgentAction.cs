namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class AgentAction : AbilityAction<IAgentSetup>
    {
        public AgentAction(
            IAbility<IAgentSetup> ability,
            int priority = 9,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use()
        {
            if (Cooldown > 0) Ability.Cooldown--;
            return false;
        }

        public override bool Use(IPlayer target)
        {
            if (Cooldown > 0)
            {
                Ability.Cooldown--;
                return false;
            }

            var detect = new Detect(Ability);
            var watch = new Watch(Ability);

            detect.Use(target);
            watch.Use(target);

            Ability.Cooldown = Setup.NightsBetweenUses;

            return true;
        }
    }

    public interface IAgentSetup : IDetectSetup, ICooldownSetup
    {
    }
}