using Mafia.NET.Localization;
using Mafia.NET.Notifications;
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

        public override IPhase? NextPhase()
        {
            return Supersedes;
        }

        public override void Start()
        {
            ChatManager.Main().Pause(false);
            var role = Notification.Popup(DayKey.ExecutionRole, Player, Player.Role);

            foreach (var player in Match.AllPlayers) player.OnNotification(role);

            if (Player.LastWill.Text.Length == 0) return;

            var lastWill1 = Notification.Chat(DayKey.LastWillAuthor, Player);
            var lastWill2 = Notification.Chat(DayKey.LastWillContent, Player.LastWill.Text);

            foreach (var player in Match.AllPlayers)
            {
                player.OnNotification(lastWill1);
                player.OnNotification(lastWill2);
            }

            base.Start();
        }

        public override void End()
        {
            base.End();

            ChatManager.Main().Pause();
            var notification = Notification.Popup(DayKey.DayEnd);
            foreach (var player in Match.AllPlayers) player.OnNotification(notification);
        }
    }
}