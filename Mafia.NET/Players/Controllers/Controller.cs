using Mafia.NET.Matches;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players.Controllers
{
    public interface IController
    {
        string Name { get; set; }
        ILobby Lobby { get; set; }

        public IPlayer Player(IMatch match, int id, IRole role);
    }

    public class Controller : IController
    {
        public Controller(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public ILobby Lobby { get; set; }

        public IPlayer Player(IMatch match, int id, IRole role)
        {
            return new Player(match, id, Name, role);
        }
    }
}