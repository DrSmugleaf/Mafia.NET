namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public abstract class GuiltyAbility<T> : BaseAbility<T> where T : IAbilitySetup, new()
    {
        protected override string GuiltyName()
        {
            return Name;
        }
    }
}