namespace Mafia.NET.Players.Roles.Abilities.Town
{
    public interface ITownAbility
    {
    }

    public abstract class TownAbility<T> : BaseAbility<T>, ITownAbility where T : ITownSetup, new()
    {
        public override bool DetectableBy(ISheriffSetup setup) => true;

        protected override string GuiltyName() => "Not Suspicious";
    }

    public interface ITownSetup : IAbilitySetup
    {
    }
}
