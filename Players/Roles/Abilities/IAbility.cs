using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; }
        IPlayer User { get; }
        string Name { get; }
        AbilityPhase Phase { get; }
        Targeting Targeting { get; }
        MessageRandomizer MurderDescriptions { get; }

        bool TryVictory(out IVictory victory);
    }
}
