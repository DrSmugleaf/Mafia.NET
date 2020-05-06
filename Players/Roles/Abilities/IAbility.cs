using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; }
        IPlayer User { get; }
        string Name { get; }
        AbilityPhase Phase { get; }
        Targeting Targeting { get; }

        bool TryVictory(out IVictory victory);
    }
}
