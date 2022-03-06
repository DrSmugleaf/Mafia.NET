using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote;

public class LastWordsPhase : BasePhase
{
    public LastWordsPhase(IMatch match, VerdictManager verdicts, uint duration = 10) : base(match, "Last Words",
        duration)
    {
        Player = verdicts.Accused;
        Verdicts = verdicts;
    }

    public IPlayer Player { get; }
    protected VerdictManager Verdicts { get; }

    public override IPhase? NextPhase()
    {
        return new ExecutionPhase(Match, Verdicts) {Supersedes = Supersedes};
    }

    public override void Start()
    {
        ChatManager.Main().Mute();
        ChatManager.Main().Mute(Player, false);
        var entry = Notification.Popup(DayKey.AnyLastWords);

        foreach (var player in Match.AllPlayers) player.OnNotification(entry);

        base.Start();
    }
}