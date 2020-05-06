using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class TrialPhase : BasePhase
    {
        public IPlayer Player { get; }

        public TrialPhase(IMatch match, IPlayer player, uint duration = 5) : base(match, "Trial", duration)
        {
            Player = player;
        }

        public override IPhase NextPhase() => new DefensePhase(Match, Player);

        public override void Start()
        {
            var notification = Notification.Popup($"The town has decided to put {Player.Name} to trial.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
