using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictResultPhase : BasePhase
    {
        private VerdictManager Verdicts { get; }

        public VerdictResultPhase(IMatch match, VerdictManager verdicts, int duration = 10) : base(match, "Vote Recount", duration)
        {
            Verdicts = verdicts;
        }

        public override void Start()
        {
            var trialOver = NotificationEventArgs.Popup("The trial is over and the votes have been counted.");
            var popup = Verdicts.Decision();
            var messages = Verdicts.Votes();

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(trialOver);
                player.OnNotification(popup);
                player.OnNotification(messages);
            }
        }

        public override IPhase End()
        {
            return Verdicts.Innocent() ? Supersedes : new LastWordsPhase(Match, Verdicts.Player);
        }
    }
}
