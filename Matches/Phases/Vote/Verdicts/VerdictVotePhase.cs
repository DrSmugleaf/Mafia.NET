using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictVotePhase : BasePhase
    {
        public VerdictManager Verdicts { get; }

        public VerdictVotePhase(IMatch match, IPlayer player, int duration = 15) : base(match, "Vote", duration)
        {
            Verdicts = new VerdictManager(match, player);
        }

        public override void Start()
        {
            NotificationEventArgs notification = NotificationEventArgs.Popup($"The town may now vote on the fate of {Verdicts.Player.Name}.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }

        public override IPhase End()
        {
            Verdicts.End();
            return new VerdictResultPhase(Match, Verdicts);
        }
    }
}
