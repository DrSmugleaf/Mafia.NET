using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictResultPhase : BasePhase
    {
        public VerdictResultPhase(IMatch match, VerdictManager verdicts, uint duration = 10) :
            base(match, "Vote Recount", duration)
        {
            Verdicts = verdicts;
        }

        protected VerdictManager Verdicts { get; }

        public override IPhase NextPhase()
        {
            return Verdicts.Innocent()
                ? Supersedes
                : new LastWordsPhase(Match, Verdicts) {Supersedes = Supersedes};
        }

        public override void Start()
        {
            var trialOver = Notification.Popup(DayKey.TrialOver);
            var decision = Verdicts.Decision();
            var messages = Verdicts.Votes();

            foreach (var player in Match.AllPlayers)
            {
                player.OnNotification(trialOver);
                player.OnNotification(decision);
                player.OnNotification(messages);
            }

            base.Start();
        }
    }
}