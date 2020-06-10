namespace Mafia.NET.Players.Roles.Abilities.Setups
{
    public interface IAbilitySetup
    {
    }

    public class EmptySetup : IAbilitySetup
    {
    }

    public interface ICooldownSetup : IAbilitySetup
    {
        int NightsBetweenUses { get; set; }
    }

    public interface IUsesSetup : IAbilitySetup
    {
        int Uses { get; set; }
    }

    public interface IRandomExcluded : IAbilitySetup
    {
        // TODO: Add to all roles
        bool ExcludedFromRandoms { get; set; }
    }
}