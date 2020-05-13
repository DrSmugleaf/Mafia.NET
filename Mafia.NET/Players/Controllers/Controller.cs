using Mafia.NET.Matches;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players.Controllers
{
    public interface IPlayerController
    {
        string Name { get; set; }
        ILobby Lobby { get; set; }

        public IPlayer Player(IMatch match, int id, IRole role);
    }

    public class PlayerController : IPlayerController
    {
        public PlayerController(string name, ILobby lobby)
        {
            Name = name;
            Lobby = lobby;
        }

        public string Name { get; set; }
        public ILobby Lobby { get; set; }

        public IPlayer Player(IMatch match, int id, IRole role)
        {
            return new Player(match, id, Name, role);
        }
    }
}