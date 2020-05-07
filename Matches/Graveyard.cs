using Mafia.NET.Players.Deaths;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches
{
    public class Graveyard
    {
        public IMatch Match { get; }
        public List<IDeath> PublicDeaths { get; }
        public List<IDeath> UndisclosedDeaths { get; }

        public Graveyard(IMatch match)
        {
            Match = match;
            PublicDeaths = new List<IDeath>();
            UndisclosedDeaths = new List<IDeath>();
        }

        public IReadOnlyList<IDeath> All() => PublicDeaths.Union(UndisclosedDeaths).ToList();

        public void Disclose()
        {
            PublicDeaths.AddRange(UndisclosedDeaths);
            UndisclosedDeaths.Clear();
        }

        public bool DiedOn(int day, DeathCause cause)
        {
            return All().Any(death => death.Day == day && death.Cause == cause);
        }

        public bool DiedToday(DeathCause cause) => DiedOn(Match.PhaseManager.Day, cause);
    }
}
