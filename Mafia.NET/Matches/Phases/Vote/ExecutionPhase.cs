using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionPhase : BasePhase
    {
        public ExecutionPhase(IMatch match, IPlayer player, uint duration = 10) : base(match, "Execution", duration)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public override IPhase NextPhase()
        {
            return new ExecutionRevealPhase(Match, Player);
        }

        public override void Start()
        {
            Player.Alive = false;
            var death = new Death(Match.Phase.Day, Player, DeathCause.LYNCH, "");
            Match.Graveyard.PublicDeaths.Add(death);
            ChatManager.Open(Match.AllPlayers);
            var notification = Notification.Popup($"May God have mercy upon your soul, {Player.Name}.");

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }

        public override void End()
        {
            base.End();

            if (Supersedes is AccusePhase accusePhase) accusePhase.Lynches--;
        }
    }
}