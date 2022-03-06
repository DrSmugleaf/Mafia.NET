using System;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Controllers;

public interface ILobbyController
{
    string Name { get; set; }
    Guid Id { get; set; }
    ILobby Lobby { get; set; }

    IPlayerController Player(IPlayer player);
}

public class LobbyController : ILobbyController
{
    public LobbyController(string name, Guid id, ILobby lobby)
    {
        Name = name;
        Id = id;
        Lobby = lobby;
    }

    public LobbyController(string name, ILobby lobby) : this(name, Guid.NewGuid(), lobby)
    {
    }

    public string Name { get; set; }
    public Guid Id { get; set; }
    public ILobby Lobby { get; set; }

    public IPlayerController Player(IPlayer player)
    {
        return new PlayerController(Name, Id, player);
    }
}