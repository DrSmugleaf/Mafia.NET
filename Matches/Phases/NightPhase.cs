using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public class NightPhase : BasePhase
    {
        public NightPhase(IMatch match, uint duration = 40) : base(match, "Night", duration)
        {
        }

        public override IPhase NextPhase() => new DeathsPhase(Match);

        public override void Start()
        {
            Match.PhaseManager.CurrentTime = TimePhase.NIGHT;
            var notification = Notification.Popup($"Night {Match.PhaseManager.Day}");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            base.Start();
        }
    }
}
