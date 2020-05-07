namespace Mafia.NET.Matches.Phases
{
    public interface IPhaseManager
    {
        IMatch Match { get; }
        int Day { get; set; }
        Time CurrentTime { get; set; }
        IPhase CurrentPhase { get; set; }
        Clock Clock { get; }

        void Start();
        void AdvancePhase();
        void SupersedePhase(IPhase newPhase);
        void Close();
    }

    public class PhaseManager : IPhaseManager
    {
        public IMatch Match { get; }
        public int Day { get; set; }
        public Time CurrentTime { get; set; }
        public IPhase CurrentPhase { get; set; }
        public Clock Clock { get; }

        public PhaseManager(IMatch match)
        {
            Match = match;
            Day = 1;
            CurrentTime = Time.DAY;
            CurrentPhase = new PresentationPhase(Match);
            Clock = new Clock();
        }

        public void Start()
        {
            CurrentPhase.Start();
            Clock.Elapsed += (source, e) => AdvancePhase();
            Clock.Start(CurrentPhase.Duration);
        }

        public void AdvancePhase()
        {
            Clock.Stop();

            var next = CurrentPhase.NextPhase();
            CurrentPhase.End();

            if (next == CurrentPhase.Supersedes)
            {
                CurrentPhase.Supersedes = null;
                next.SupersededBy = null;
                next.Resume();
            }
            else
            {
                next.Start();
            }

            Clock.Start(next.Duration);
        }

        public void SupersedePhase(IPhase newPhase)
        {
            Clock.Stop();

            CurrentPhase.SupersededBy = newPhase;
            newPhase.Supersedes = CurrentPhase;

            CurrentPhase.Pause();

            CurrentPhase = newPhase;
            CurrentPhase.Start();

            Clock.Start(CurrentPhase.Duration);
        }

        public void Close()
        {
            Clock.Stop();
            Clock.Dispose();
        }
    }
}
