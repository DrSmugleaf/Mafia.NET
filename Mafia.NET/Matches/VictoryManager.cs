using System.Collections.Generic;

namespace Mafia.NET.Matches
{
    public interface IVictoryManager
    {
        IMatch Match { get; }
    }

    public class VictoryManager : IVictoryManager
    {
        public VictoryManager(IMatch match)
        {
            Match = match;
        }

        public IMatch Match { get; }

        public bool TryVictory(out IList<IVictory> victories) // TODO better victory checking
        {
            victories = new List<IVictory>();

            foreach (var player in Match.AllPlayers)
                if (player.Role.Ability.TryVictory(out var victory))
                    victories.Add(victory);

            return victories.Count > 0;
        }
    }
}