using Mafia.NET.Matches.Chats;
using System;
using System.Timers;

#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public abstract class BasePhase : IPhase
    {
        public IMatch Match { get; }
        public string Name { get; }
        public int Duration { get; }
        public IPhase? Supersedes { get; set; }
        public IPhase? SupersededBy { get; set; }
        public bool Skippable { get; }
        public ChatManager ChatManager { get; }
        public Timer Timer { get; protected set; }

        public BasePhase(IMatch match, string name, int duration = -1, bool skippable = false)
        {
            Match = match;
            Name = name;
            Duration = duration;
            Skippable = skippable;
            ChatManager = new ChatManager();
            Timer = new Timer();
        }

        public abstract IPhase NextPhase();

        public virtual void Start()
        {
            Timer.Interval = Duration * 1000;
            Timer.Elapsed += (source, e) => End();
            Timer.AutoReset = false;
            Timer.Start();
        }

        public virtual void End()
        {
            Timer.Stop();
            Timer.Dispose();
            ChatManager.Close();
        }
    }
}
