using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches.Options;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public interface ILobby
    {
        Setup Setup { get; set; }
        IPlayerController Host { get; set; }
        IReadOnlyList<IPlayerController> Controllers { get; }

        IPlayerController Add(string name);
        IMatch Start();
    }

    public class Lobby : ILobby
    {
        private readonly List<IPlayerController> _controllers;

        public Lobby(string hostName, Setup setup = null)
        {
            Setup = setup ?? new Setup();
            Host = new PlayerController(hostName, this);
            _controllers = new List<IPlayerController>();
        }
        
        public Setup Setup { get; set; }
        public IPlayerController Host { get; set; }
        public IReadOnlyList<IPlayerController> Controllers => _controllers;

        public IPlayerController Add(string name)
        {
            var controller = new PlayerController(name, this);
            _controllers.Add(controller);
            return controller;
        }
        
        public IMatch Start()
        {
            return new Match(Setup, _controllers);
        }
    }
}