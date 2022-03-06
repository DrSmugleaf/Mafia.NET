using Mafia.NET.Matches.Phases.Vote;

namespace Mafia.NET.Matches.Phases;

public class DiscussionPhase : BasePhase
{
    public DiscussionPhase(IMatch match, uint duration = 50) :
        base(match, "Discussion", duration)
    {
    }

    public override IPhase? NextPhase()
    {
        return new AccusePhase(Match);
    }

    public override void Start()
    {
        ChatManager.Main().Pause(false);
        foreach (var player in Match.AllPlayers) player.OnDayStart();
        Match.Executor.OnDayStart();

        base.Start();
    }
}