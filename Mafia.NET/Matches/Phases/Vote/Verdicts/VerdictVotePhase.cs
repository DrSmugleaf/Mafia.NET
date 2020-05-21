using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Abilities.Mafia;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictVotePhase : BasePhase
    {
        public VerdictVotePhase(IMatch match, IPlayer player, uint duration = 15) : base(match, "Vote", duration)
        {
            Verdicts = new VerdictManager(match, player);
        }

        public VerdictManager Verdicts { get; }

        public override IPhase NextPhase()
        {
            return new VerdictResultPhase(Match, Verdicts);
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers);

            if (Verdicts.Player.Blackmailed &&
                !Match.Setup.Roles.Abilities.Setup<BlackmailerSetup>().BlackmailedTalkDuringTrial)
                ChatManager.Main().Participants[Verdicts.Player].Muted = true;

            var notification = Notification.Popup(DayKey.MayVote, Verdicts.Player.Name);

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }

        public override void End()
        {
            base.End();
            Verdicts.End();
        }
    }
}