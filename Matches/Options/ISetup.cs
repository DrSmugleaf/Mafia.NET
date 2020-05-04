using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Matches.Options
{
    public interface ISetup
    {
        IPhase Procedure { get; }
        bool AnonymousVoting { get; }
        RoleSetup Roles { get; }
    }
}
