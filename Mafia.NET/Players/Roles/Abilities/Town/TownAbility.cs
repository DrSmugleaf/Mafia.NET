namespace Mafia.NET.Players.Roles.Abilities.Town
{
    public interface ITownAbility
    {
    }

    public abstract class TownAbility<T> : BaseAbility<T>, ITownAbility where T : ITownSetup, new()
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return true;
        }

        protected override string GuiltyName()
        {
            return "Not Suspicious";
        }
    }

    public interface ITownSetup : IAbilitySetup
    {
    }
}