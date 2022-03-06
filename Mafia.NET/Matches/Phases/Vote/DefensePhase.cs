using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Notifications;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Abilities;

namespace Mafia.NET.Matches.Phases.Vote;

public class DefensePhase : BasePhase
{
    public DefensePhase(IMatch match, IPlayer accused, uint duration = 15) :
        base(match, "Defense", duration)
    {
        Accused = accused;
    }

    public IPlayer Accused { get; }

    public override IPhase? NextPhase()
    {
        return new VerdictVotePhase(Match, Accused) {Supersedes = Supersedes};
    }

    public bool CanTalk(IPlayer player)
    {
        return player == Accused &&
               (!player.Perks.Blackmailed ||
                player.Perks.Blackmailers
                    .Select(bm => bm.Abilities.Get<Blackmail>())
                    .Where(ability => ability != null)
                    .All(ability => ability!.Setup.TalkDuringTrial));
    }

    public override void Start()
    {
        foreach (var player in Match.AllPlayers)
            if (player != Accused)
                ChatManager.Main().Get(player).Pause();

        if (CanTalk(Accused)) ChatManager.Main().Mute(Accused, false);

        ChatManager.Main().Pause(false);

        var notification = Notification.Popup(DayKey.OnTrial, Accused);
        foreach (var player in Match.AllPlayers) player.OnNotification(notification);

        base.Start();
    }

    public override void End()
    {
        base.End();

        foreach (var player in Match.AllPlayers)
            ChatManager.Main().Get(player).Pause(false);
    }
}