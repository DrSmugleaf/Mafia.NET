using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class TrialPhase : BasePhase
    {
        public IPlayer Player { get; }

        public TrialPhase(IMatch match, IPlayer player) : base(match, "Trial", nextPhase: new DefensePhase(match, player))
        {
        }

        public override void Start()
        {
            var notification = NotificationEventArgs.Popup($"The town has decided to put {Player.Name} to trial.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
