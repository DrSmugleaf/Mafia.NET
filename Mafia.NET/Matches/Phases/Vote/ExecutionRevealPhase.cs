using System;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionRevealPhase : BasePhase
    {
        public IPlayer Player;

        public ExecutionRevealPhase(IMatch match, IPlayer player, uint duration = 10) : base(match, "Execution Reveal",
            duration)
        {
            Player = player;
        }

        public override IPhase NextPhase()
        {
            return Supersedes;
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers);
            var role = Notification.Popup($"{Player.Name}'s role was {Player.Role.Name}.");

            foreach (var player in Match.AllPlayers) player.OnNotification(role);

            if (Player.LastWill.Text.Length == 0) return;

            var lastWill =
                Notification.Chat($"{Player.Name} left us his last will:{Environment.NewLine}{Player.LastWill}");

            foreach (var player in Match.AllPlayers) player.OnNotification(lastWill);

            base.Start();
        }

        public override void End()
        {
            base.End();

            var notification = Notification.Popup("Let us reconvene tomorrow.");

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);
        }
    }
}