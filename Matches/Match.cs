using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Options;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Matches.Phases.Vote;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

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
        public IList<IChat> OpenChats { get; }
        public Timer Timer { get; }

        public Match(ISetup settings, Dictionary<int, IPlayer> players, List<IRole> possibleRoles)
        {
            Setup = settings;
            AllPlayers = players;
            Graveyard = new List<IDeath>();
            PossibleRoles = possibleRoles;
            Day = 0;
            CurrentTime = TimePhase.DAY;
            CurrentPhase = new PresentationPhase(this);
            OpenChats = new List<IChat>();
            Timer = new Timer(CurrentPhase.DurationMs);
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
            CurrentPhase = CurrentPhase.End();
            CurrentPhase.Start();
        }

        public void End()
        {
            Timer.Stop();
        }

        protected virtual void ProcedureStarted(object sender, ProcedureStartEventArgs e)
        {
            SupersedePhase(e.Phase);
        }
    }
}
