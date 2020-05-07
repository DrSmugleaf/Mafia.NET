using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; set; }
        IPlayer User { get; set; }
        string Name { get; set; }
        Targeting Targeting { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        IAbilitySetup AbilitySetup { get; }

        bool TryVictory(out IVictory victory);
    }
}
