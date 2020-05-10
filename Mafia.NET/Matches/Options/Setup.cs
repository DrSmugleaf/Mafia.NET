using Mafia.NET.Players.Roles;

namespace Mafia.NET.Matches.Options
{
    public class Setup : ISetup
    {
        public Setup(RoleSetup roles, bool trial = true, bool anonymousVoting = false)
        {
            Roles = roles;
            Trial = trial;
            AnonymousVoting = anonymousVoting;
        }

        public RoleSetup Roles { get; }
        public bool Trial { get; }
        public bool AnonymousVoting { get; }
    }
}