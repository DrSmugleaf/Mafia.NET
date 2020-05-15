using Mafia.NET.Matches;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players.Controllers
{
    public interface ILobbyController
    {
        string Name { get; set; }
        string Id { get; set; }
        ILobby Lobby { get; set; }

        IPlayerController Player(IPlayer player);
    }
    
    public class LobbyController : ILobbyController
    {
        public LobbyController(string name, string id, ILobby lobby)
        {
            Name = name;
            Id = id;
            Lobby = lobby;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public ILobby Lobby { get; set; }

        public IPlayerController Player(IPlayer player)
        {
            return new PlayerController(Name, Id, player);
        }
    }
}