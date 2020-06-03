namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Release : AbilityAction
    {
        public Release(
            IAbility ability,
            int priority = -2,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
            Filter = action =>
                action.TargetManager.Try(out var target) &&
                action.TargetManager.TryDay(out var prisoner) &&
                target == prisoner;
        }

        public override bool Use()
        {
            if (!Ability.TargetManager.TryDay(out var prisoner)) return false;

            prisoner.Ability.PiercingBlockedBy(User);
            return true;
        }
    }
}