using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Notifications;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Abilities.Mafia;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class DefensePhase : BasePhase
    {
        public DefensePhase(IMatch match, IPlayer accused, uint duration = 15) : base(match, "Defense", duration)
        {
            Accused = accused;
        }

        public IPlayer Accused { get; }

        public override IPhase NextPhase()
        {
            return new VerdictVotePhase(Match, Accused) {Supersedes = Supersedes};
        }

        public override void Start()
        {
            foreach (var player in Match.AllPlayers)
                if (player != Accused)
                    ChatManager.Main().Get(player).Pause();

            if (!Accused.Blackmailed || Match.AbilitySetups.Setup<BlackmailerSetup>().BlackmailedTalkDuringTrial)
                ChatManager.Main().Mute(Accused, false);

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
}