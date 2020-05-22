using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Selectors;

namespace Mafia.NET.Matches.Options
{
    public interface ISetup
    {
        bool Trial { get; set; }
        bool AnonymousVoting { get; set; }
        RoleSetup Roles { get; set; }
    }
}