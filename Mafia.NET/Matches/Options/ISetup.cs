using Mafia.NET.Players.Roles.Selectors;

namespace Mafia.NET.Matches.Options
{
    public interface ISetup
    {
        bool Trial { get; }
        bool AnonymousVoting { get; }
        RoleSetup Roles { get; }
    }
}