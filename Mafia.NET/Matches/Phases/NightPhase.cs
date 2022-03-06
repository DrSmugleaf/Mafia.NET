using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Matches.Phases;

public class NightPhase : BasePhase
{
    public NightPhase(IMatch match, uint duration = 40) :
        base(match, "Night", duration)
    {
    }

    public override IPhase? NextPhase()
    {
        return new DeathsPhase(Match);
    }

    public override void Start()
    {
        ChatManager.Main().Pause();
        Match.Phase.CurrentTime = Time.Night;
        var notification = Notification.Popup(DayKey.Night, Match.Phase.Day);

        foreach (var player in Match.AllPlayers)
        {
            player.OnNotification(notification);
            player.OnNightStart();
        }

        Match.Executor.OnNightStart();

        base.Start();
    }

    public override void End()
    {
        base.End();
        foreach (var player in Match.AllPlayers) player.BeforeNightEnd();
        Match.Executor.OnNightEnd();
        foreach (var player in Match.AllPlayers) player.OnNightEnd();
    }
}