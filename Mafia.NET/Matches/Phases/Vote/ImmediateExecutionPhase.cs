using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ImmediateExecutionPhase : BasePhase
    {
        public IPlayer Player { get; }

        public ImmediateExecutionPhase(IMatch match, IPlayer player, uint duration = 5) : base(match, "Immediate Execution", duration)
        {
            Player = player;
        }

        public override IPhase NextPhase() => new ExecutionPhase(Match, Player);

        public override void Start()
        {
            var notification = Notification.Popup($"The town will execute {Player.Name} next.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
