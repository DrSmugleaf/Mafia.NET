using Mafia.NET.Players.Roles;

namespace Mafia.NET.Matches.Options
{
    public interface ISetup
    {
        bool Trial { get; set; }
        bool AnonymousVoting { get; set; }
        RoleSetup Roles { get; set; }
    }
}