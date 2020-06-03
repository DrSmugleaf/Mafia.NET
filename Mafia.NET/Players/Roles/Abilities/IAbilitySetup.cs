namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbilitySetup
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

    public interface INightImmune : IAbilitySetup
    {
        int NightImmunity { get; set; }
    }

    public interface IRoleBlockImmune : IAbilitySetup
    {
        bool RoleBlockImmune { get; set; }
    }

    public interface IDetectionImmune : IAbilitySetup
    {
        bool DetectionImmune { get; set; }
    }

    public interface IIgnoresDetectionImmunity : IAbilitySetup
    {
        bool IgnoresDetectionImmunity { get; set; }
    }

    public interface IRandomExcluded : IAbilitySetup
    {
        bool ExcludedFromRandoms { get; set; }
    }
}