using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    public interface ITownAbility
    {
    }

    public abstract class TownAbility<T> : BaseAbility<T>, ITownAbility where T : class, ITownSetup, new()
    {
        public override bool DetectableBy(ISheriffSetup setup = null)
        {
            return true;
        }

        public override Key GuiltyName()
        {
            return SheriffKey.NotSuspicious;
        }
    }

    public interface ITownSetup : IAbilitySetup
    {
    }
}