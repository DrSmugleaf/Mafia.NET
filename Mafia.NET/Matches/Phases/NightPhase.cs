using Mafia.NET.Localization;

namespace Mafia.NET.Matches.Phases
{
    public class NightPhase : BasePhase
    {
        public NightPhase(IMatch match, uint duration = 40) : base(match, "Night", duration)
        {
        }

        public override IPhase NextPhase()
        {
            return new DeathsPhase(Match);
        }

        public override void Start()
        {
            Match.Phase.CurrentTime = Time.Night;
            var notification = Entry.Popup(DayKey.Night, Match.Phase.Day);

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }
    }
}