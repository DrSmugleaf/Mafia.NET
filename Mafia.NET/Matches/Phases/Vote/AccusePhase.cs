using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class AccusePhase : BasePhase
    {
        public AccusePhase(IMatch match, uint duration = 80) : base(match, "Time Left", duration, true)
        {
            Trial = match.Setup.Trial;
            Lynches = 1;
        }

        public AccuseManager AccuseManager { get; protected set; }
        public bool Trial { get; set; }
        public int Lynches { get; set; }

        protected void StartTrial(IPlayer accused)
        {
            var phase = Trial
                ? (IPhase) new TrialPhase(Match, accused) {Supersedes = this}
                : new ImmediateExecutionPhase(Match, accused) {Supersedes = this};

            Match.Phase.SupersedePhase(phase);
        }

        public override IPhase NextPhase()
        {
            return new NightPhase(Match);
        }

        public override void Start()
        {
            AccuseManager = new AccuseManager(Match, StartTrial);

            var notification = Notification.Popup(DayKey.VotingBegin);
            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            notification = Notification.Popup(DayKey.VotesNeeded, Match.LivingPlayers.Count / 2 + 1);
            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }

        public override void Pause()
        {
            AccuseManager.Active = false;
            base.Pause();
        }

        public override double Resume()
        {
            if (Lynches > 0)
            {
                AccuseManager = new AccuseManager(Match, StartTrial);
                return base.Resume();
            }

            return 0;
        }

        public override void End()
        {
            AccuseManager.Active = false;
            base.End();

            Match.Actions.OnDayEnd();
        }
    }
}