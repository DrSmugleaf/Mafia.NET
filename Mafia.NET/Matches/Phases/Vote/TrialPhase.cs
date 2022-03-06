using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote;

public class TrialPhase : BasePhase
{
    public TrialPhase(IMatch match, IPlayer accused, uint duration = 5) : base(match, "Trial", duration)
    {
        Accused = accused;
    }

    public IPlayer Accused { get; }

    public override IPhase? NextPhase()
    {
        return new DefensePhase(Match, Accused) {Supersedes = Supersedes};
    }

    public override void Start()
    {
        var entry = Notification.Popup(DayKey.PutToTrial, Accused);

        foreach (var player in Match.AllPlayers) player.OnNotification(entry);

        base.Start();
    }
}