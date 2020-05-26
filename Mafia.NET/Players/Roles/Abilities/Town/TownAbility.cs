using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    public interface ITownAbility
    {
    }

    public abstract class TownAbility<T> : BaseAbility<T>, ITownAbility where T : class, ITownSetup, new()
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return true;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.NotSuspicious;
        }
    }

    public interface ITownSetup : IAbilitySetup
    {
    }
}