using Mafia.NET.Player.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia.NET.Player
{
    interface IPlayer
    {
        string Name { get; set; }
        IRole Role { get; set; }
    }
}
