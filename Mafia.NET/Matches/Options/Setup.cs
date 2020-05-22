using Mafia.NET.Players.Roles.Selectors;

namespace Mafia.NET.Matches.Options
{
    public class Setup : ISetup
    {
        public Setup(RoleSetup roles = null, bool trial = true, bool anonymousVoting = false)
        {
            Roles = roles ?? new RoleSetup();
            Trial = trial;
            AnonymousVoting = anonymousVoting;
        }

        public RoleSetup Roles { get; set; }
        public bool Trial { get; set; }
        public bool AnonymousVoting { get; set; }
    }
}