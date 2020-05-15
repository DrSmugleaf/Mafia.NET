using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Matches.Options;
using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Matches
{
    public interface ILobby
    {
        string Id { get; }
        Setup Setup { get; set; }
        ILobbyController Host { get; set; }
        IReadOnlyList<ILobbyController> Controllers { get; }
        bool Started { get; }

        ILobbyController Add(string name, string id);
        IMatch Start();
    }

    public class Lobby : ILobby
    {
        private readonly List<ILobbyController> _controllers;

        public Lobby(string id, string hostName, string hostId, Setup setup = null)
        {
            Id = id;
            Setup = setup ?? new Setup();
            Host = new LobbyController(hostName, hostId, this);
            _controllers = new List<ILobbyController> {Host};
            Started = false;
        }

        public string Id { get; }
        public Setup Setup { get; set; }
        public ILobbyController Host { get; set; }
        public IReadOnlyList<ILobbyController> Controllers => _controllers;
        public bool Started { get; protected set; }
        [CanBeNull] protected IMatch Match { get; set; }

        public ILobbyController Add(string name, string id)
        {
            var controller = new LobbyController(name, id, this);
            _controllers.Add(controller);
            return controller;
        }
        
        public IMatch Start()
        {
            if (Started) return Match;
            
            Started = true;
            Match = new Match(Setup, _controllers);
            return Match;
        }
    }
}