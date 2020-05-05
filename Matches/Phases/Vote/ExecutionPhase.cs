using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionPhase : BasePhase
    {
        public IPlayer Player { get; }

        public ExecutionPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Execution", duration, new ExecutionRevealPhase(match, player))
        {
            Player = player;
        }

        public override void Start()
        {
            Player.Alive = false;
            var death = new Death(Match.Day, Player, DeathCause.LYNCH);
            Match.Graveyard.Add(death);
            ChatManager.Open(Match.AllPlayers.Values);
            Notification notification = Notification.Popup($"May God have mercy upon your soul, {Player.Name}.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
