using System;

namespace Mafia.NET.Web.Models
{
    public class JoinGameViewModel
    {
        public string Name { get; set; }
        public string Game { get; set; }

        public Guid GameGuid()
        {
            return Guid.Parse(Game.Trim());
        }
        
        public bool IsValidCreate()
        {
            return Name.Length < 31 && Name.Length > 2;
        }

        public bool IsValidJoin()
        {
            return IsValidCreate() && Guid.TryParse(Game, out _);
        }
    }
}