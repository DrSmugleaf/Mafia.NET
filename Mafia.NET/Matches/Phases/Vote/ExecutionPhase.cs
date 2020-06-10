using Mafia.NET.Localization;
using Mafia.NET.Notifications;
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
            return new ExecutionRevealPhase(Match, Player) {Supersedes = Supersedes};
        }

        public override void Start()
        {
            Player.Alive = false; // TODO: Ability activity
            var death = new Death(Match.Phase.Day, Player, DeathCause.Lynch, "");
            Match.Graveyard.PublicDeaths.Add(death);
            ChatManager.Main().Get(Player).Muted = true;
            ChatManager.Main().Pause(false);
            var notification = Notification.Popup(DayKey.ExecutionMercy, Player);

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