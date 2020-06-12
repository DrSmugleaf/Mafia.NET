namespace Mafia.NET.Players.Roles.Abilities.Setups
{
    public interface IAbilitySetup
    {
    }

    [RegisterSetup]
    public class EmptySetup : IAbilitySetup
    {
    }

    public interface ICooldownSetup : IAbilitySetup
    {
        int NightsBetweenUses { get; set; }
    }

    public interface IRandomExcluded : IAbilitySetup
    {
        // TODO: Add to all roles
        bool ExcludedFromRandoms { get; set; }
    }
}