using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public interface ILobby
    {
        Guid Id { get; }
        Setup Setup { get; set; }
        ILobbyController Host { get; set; }
        IReadOnlyList<ILobbyController> Controllers { get; }
        bool Started { get; }

        IEnumerable<ILobbyController> Except(ILobbyController controller);
        ILobbyController Add(string name, Guid id);
        IMatch Start();
    }

    public class Lobby : ILobby
    {
        private readonly List<ILobbyController> _controllers;

        public Lobby(Guid id, string hostName, Guid hostId, Setup? setup = null)
        {
            Id = id;
            Setup = setup ?? new Setup();
            Host = new LobbyController(hostName, hostId, this);
            _controllers = new List<ILobbyController> {Host};
            Started = false;
        }

        protected IMatch? Match { get; set; }

        public Guid Id { get; }
        public Setup Setup { get; set; }
        public ILobbyController Host { get; set; }
        public IReadOnlyList<ILobbyController> Controllers => _controllers;
        public bool Started { get; protected set; }

        public IEnumerable<ILobbyController> Except(ILobbyController except)
        {
            return Controllers.Where(controller => controller != except);
        }

        public ILobbyController Add(string name, Guid id)
        {
            var controller = new LobbyController(name, id, this);
            _controllers.Add(controller);
            return controller;
        }

        public IMatch Start()
        {
            if (Started) return Match!;

            Started = true;
            Match = new Match(Id, Setup, _controllers);
            return Match;
        }
    }
}