using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ImmediateExecutionPhase : BasePhase
    {
        public ImmediateExecutionPhase(IMatch match, IPlayer player, uint duration = 5) : base(match,
            "Immediate Execution", duration)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public override IPhase NextPhase()
        {
            var verdicts = new VerdictManager(Player);
            verdicts.End();

            return new ExecutionPhase(Match, verdicts) {Supersedes = Supersedes};
        }

        public override void Start()
        {
            var popup = Notification.Popup(DayKey.ImmediateExecution, Player);
            foreach (var player in Match.AllPlayers) player.OnNotification(popup);

            base.Start();
        }
    }
}