using System.Timers;

namespace Mafia.NET.Matches.Phases
{
    public class Clock : Timer
    {
        public Clock()
        {
        }

        public Clock(double interval) : base(interval)
        {
        }

        public new double Interval
        {
            get => base.Interval;
            set
            {
                var old = Enabled;
                Enabled = true;
                base.Interval = value;
                Enabled = old;
            }
        }

        public void Start(double interval)
        {
            Interval = interval;
            Start();
        }
    }
}