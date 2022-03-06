using Mafia.NET.Players.Roles.Selectors;

namespace Mafia.NET.Matches;

public interface ISetup
{
    bool Trial { get; }
    bool AnonymousVoting { get; }
    RoleSetup Roles { get; }
}

public class Setup : ISetup
{
    public Setup(RoleSetup? roles = null, bool trial = true, bool anonymousVoting = false)
    {
        Trial = trial;
        AnonymousVoting = anonymousVoting;
        Roles = roles ?? new RoleSetup();
    }

    public bool Trial { get; set; }
    public bool AnonymousVoting { get; set; }
    public RoleSetup Roles { get; set; }
}