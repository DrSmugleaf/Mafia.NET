using Mafia.NET.Matches;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Players.Controllers
{
    public interface IPlayerController
    {
        string Name { get; set; }
        string Id { get; set; }
        IPlayer Player { get; set; }
    }

    public class PlayerController : IPlayerController
    {
        public PlayerController(string name, string id, IPlayer player)
        {
            Name = name;
            Id = id;
            Player = player;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public IPlayer Player { get; set; }
    }
}