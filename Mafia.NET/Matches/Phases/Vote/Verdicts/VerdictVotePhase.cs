using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts;

public class VerdictVotePhase : BasePhase
{
    public VerdictVotePhase(IMatch match, IPlayer player, uint duration = 15) : base(match, "Vote", duration)
    {
        Verdicts = new VerdictManager(player);
    }

    public VerdictManager Verdicts { get; }

    public override IPhase? NextPhase()
    {
        return new VerdictResultPhase(Match, Verdicts) {Supersedes = Supersedes};
    }

    public override void Start()
    {
        ChatManager.Main().Pause(false);

        var notification = Notification.Popup(DayKey.MayVote, Verdicts.Accused);
        foreach (var player in Match.AllPlayers) player.OnNotification(notification);

        base.Start();
    }

    public override void End()
    {
        base.End();
        Verdicts.End();
    }
}