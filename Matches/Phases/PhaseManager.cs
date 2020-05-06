using System.Timers;

namespace Mafia.NET.Matches.Phases
{
    public interface IPhaseManager
    {
        IMatch Match { get; }
        int Day { get; set; }
        TimePhase CurrentTime { get; set; }
        IPhase CurrentPhase { get; set; }
        Timer Timer { get; }

        void Start();
        void AdvancePhase();
        void SupersedePhase(IPhase newPhase);
        void Close();
    }

    public class PhaseManager : IPhaseManager
    {
        public IMatch Match { get; }
        public int Day { get; set; }
        public TimePhase CurrentTime { get; set; }
        public IPhase CurrentPhase { get; set; }
        public Timer Timer { get; }

        public PhaseManager(IMatch match)
        {
            Match = match;
            Day = 1;
            CurrentTime = TimePhase.DAY;
            CurrentPhase = new PresentationPhase(Match);
            Timer = new Timer();
        }

        public void Start()
        {
            CurrentPhase.Start();
            Timer.Interval = CurrentPhase.Duration;
            Timer.Elapsed += (source, e) => AdvancePhase();
            Timer.AutoReset = false;
            Timer.Start();
        }

        public void AdvancePhase()
        {
            Timer.Stop();
            var next = CurrentPhase.NextPhase();
            CurrentPhase.End();

            if (next == CurrentPhase.Supersedes)
            {
                CurrentPhase.Supersedes = null;
                next.SupersededBy = null;
                Timer.Interval = next.Duration;
                next.Resume();
            }
            else
            {
                Timer.Interval = next.Duration;
                next.Start();
            }

            Timer.Start();
        }

        public void SupersedePhase(IPhase newPhase)
        {
            Timer.Stop();

            CurrentPhase.SupersededBy = newPhase;
            newPhase.Supersedes = CurrentPhase;

            CurrentPhase.Pause();

            CurrentPhase = newPhase;
            CurrentPhase.Start();

            Timer.Interval = CurrentPhase.Duration;
            Timer.Start();
        }

        public void Close()
        {
            Timer.Stop();
            Timer.Dispose();
        }
    }
}
