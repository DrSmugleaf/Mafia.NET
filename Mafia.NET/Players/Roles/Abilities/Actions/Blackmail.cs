namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Blackmail : AbilityAction<IBlackmailSetup>
    {
        public Blackmail(
            IAbility<IBlackmailSetup> ability,
            int priority = 4,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            target.Blackmailed = true;
            return true;
        }
    }

    public interface IBlackmailSetup : IAbilitySetup
    {
        public bool BlackmailedTalkDuringTrial { get; set; }
    }
}