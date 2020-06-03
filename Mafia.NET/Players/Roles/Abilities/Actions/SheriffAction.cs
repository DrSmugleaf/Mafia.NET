namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class SheriffAction : AbilityAction<ISheriffSetup>
    {
        public SheriffAction(
            IAbility<ISheriffSetup> ability,
            int priority = 9,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            var message = target.Crimes.Sheriff(Setup).Chat();
            User.OnNotification(message);

            return true;
        }
    }

    public interface ISheriffSetup : IAbilitySetup
    {
        bool DetectsMafiaTriad { get; set; }
        bool DetectsSerialKiller { get; set; }
        bool DetectsArsonist { get; set; }
        bool DetectsCult { get; set; }
        bool DetectsMassMurderer { get; set; }
    }
}