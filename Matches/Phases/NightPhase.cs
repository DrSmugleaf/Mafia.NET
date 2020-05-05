using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public class NightPhase : BasePhase
    {
        public NightPhase(IMatch match, int duration = 40) : base(match, "Night", duration, new DeathsPhase(match))
        {
        }

        public override void Start()
        {
            var notification = Notification.Popup($"Night {Match.Day}");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }
    }
}
