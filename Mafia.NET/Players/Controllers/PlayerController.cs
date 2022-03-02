using System;

namespace Mafia.NET.Players.Controllers
{
    public interface IPlayerController
    {
        string Name { get; set; }
        Guid Id { get; set; }
        IPlayer Player { get; set; }
    }

    public class PlayerController : IPlayerController
    {
        private IPlayer _player;

        public PlayerController(string name, Guid id, IPlayer player)
        {
            Name = name;
            Id = id;
            _player = player;
            Player = player;
            player.Controller = this;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public IPlayer Player
        {
            get => _player;
            set
            {
                _player = value;
                _player.Controller = this;
            }
        }
    }
}