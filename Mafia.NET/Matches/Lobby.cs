using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches.Options;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public interface ILobby
    {
        Setup Setup { get; set; }
        List<IController> Controllers { get; set; }

        IMatch Start();
    }

    public class Lobby : ILobby
    {
        public Lobby(Setup setup, IList<IController> controllers)
        {
            Setup = setup;
            Controllers = controllers.ToList();
        }

        public Setup Setup { get; set; }
        public List<IController> Controllers { get; set; }

        public IMatch Start()
        {
            return new Match(Setup, Controllers);
        }
    }
}