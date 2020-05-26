using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public abstract class GuiltyAbility<T> : BaseAbility<T> where T : class, IAbilitySetup, new()
    {
        protected override Key GuiltyName()
        {
            return User.Role.Name;
        }
    }
}