using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles;

namespace Mafia.NET.Matches.Options
{
    public class Setup : ISetup
    {
        public IPhase Procedure { get; }
        public bool AnonymousVoting { get; }
        public RoleSetup Roles { get; }

        public Setup(IPhase procedure, bool anonymousVoting, RoleSetup roles)
        {
            Procedure = procedure;
            AnonymousVoting = anonymousVoting;
            Roles = roles;
        }
    }
}
