using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Matches
{
    public class Match : IMatch
    {
        public ISetup Setup { get; }
        public IReadOnlyDictionary<int, IPlayer> AllPlayers { get; }
        public IReadOnlyDictionary<int, IPlayer> LivingPlayers => new Dictionary<int, IPlayer>(AllPlayers.Where(player => player.Value.Alive));
        public List<IDeath> Graveyard { get; }
        public IList<IDeath> UndisclosedDeaths { get; }
        public IReadOnlyList<IRole> PossibleRoles { get; }
        public int Day { get; set; }
        public TimePhase CurrentTime { get; set; }
        public IPhase CurrentPhase { get; set; }

        public Match(ISetup settings, Dictionary<int, IPlayer> players, List<IRole> possibleRoles)
        {
            Setup = settings;
            AllPlayers = players;
            Graveyard = new List<IDeath>();
            PossibleRoles = possibleRoles;
            Day = 0;
            CurrentTime = TimePhase.DAY;
            CurrentPhase = new PresentationPhase(this);
        }

        public void SupersedePhase(IPhase newPhase)
        {
            CurrentPhase.SupersededBy = newPhase;
            newPhase.Supersedes = CurrentPhase;
            CurrentPhase = newPhase;
            CurrentPhase.Start();
        }

        public void AdvancePhase(object state)
        {
            CurrentPhase = CurrentPhase.NextPhase();
            CurrentPhase.Start();
        }

        public void Start() => CurrentPhase.Start();

        public void End() => Expression.Empty();
    }
}
