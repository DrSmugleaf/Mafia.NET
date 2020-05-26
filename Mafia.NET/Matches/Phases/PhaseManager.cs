using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public interface IPhaseManager
    {
        IMatch Match { get; }
        int Day { get; set; }
        Time CurrentTime { get; set; }
        IPhase CurrentPhase { get; set; }
        Clock Clock { get; }
        ChatManager Chat { get; }

        void Start();
        void AdvancePhase();
        void SupersedePhase(IPhase newPhase);
        void Close();
    }

    public class PhaseManager : IPhaseManager
    {
        public PhaseManager(IMatch match)
        {
            Match = match;
            Day = 1;
            CurrentTime = Time.Day;
            CurrentPhase = new PresentationPhase(Match);
            Clock = new Clock();
            Chat = new ChatManager(match);
        }

        public IMatch Match { get; }
        public int Day { get; set; }
        public Time CurrentTime { get; set; }
        public IPhase CurrentPhase { get; set; }
        public Clock Clock { get; }
        public ChatManager Chat { get; }

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

            double duration;
            if (next == CurrentPhase.Supersedes)
            {
                CurrentPhase.Supersedes = null;
                next.SupersededBy = null;
                duration = next.Resume();
            }
            else
            {
                next.Start();
                duration = next.Duration;
            }

            CurrentPhase = next;

            if (duration > 0)
                Clock.Start(next.Duration);
            else
                AdvancePhase();
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