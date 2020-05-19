using System;

namespace Mafia.NET.Web.Models
{
    public class JoinGameViewModel
    {
        public string Name { get; set; }
        public string Lobby { get; set; }

        public Guid LobbyGuid()
        {
            return Guid.Parse(Lobby.Trim());
        }

        public bool IsValidCreate()
        {
            return Name.Length < 31 && Name.Length > 2;
        }

        public bool IsValidJoin()
        {
            return IsValidCreate() && Guid.TryParse(Lobby, out _);
        }
    }
}