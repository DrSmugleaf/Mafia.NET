using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class DefensePhase : BasePhase
    {
        public IPlayer Player { get; }

        public DefensePhase(IMatch match, IPlayer player, uint duration = 15) : base(match, "Defense", duration)
        {
            Player = player;
        }

        public override IPhase NextPhase()
        {
            return new VerdictVotePhase(Match, Player);
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values, true);
            ChatManager.Main().Participants[Player].Muted = false;

            var notification = Notification.Popup($"{Player.Name}, you are on trial for conspiracy against the town. What is your defense?");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
