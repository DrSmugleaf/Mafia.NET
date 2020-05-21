using Mafia.NET.Localization;
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
            return new ExecutionPhase(Match, Player);
        }

        public override void Start()
        {
            var entry = Notification.Popup(DayKey.ImmediateExecution, Player);

            foreach (var player in Match.AllPlayers) player.OnNotification(entry);

            base.Start();
        }
    }
}