using Mafia.NET.Matches.Chats;
using System;

#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public abstract class BasePhase : IPhase
    {
        public IMatch Match { get; }
        public string Name { get; }
        public double Duration { get; protected set; }
        public DateTime StartTime { get; protected set; }
        public double Elapsed { get; protected set; }
        public IPhase? Supersedes { get; set; }
        public IPhase? SupersededBy { get; set; }
        public bool Skippable { get; }
        public ChatManager ChatManager { get; }

        public BasePhase(IMatch match, string name, uint duration, bool skippable = false)
        {
            Match = match;
            Name = name;
            Duration = duration * 1000;
            Skippable = skippable;
            ChatManager = new ChatManager();
        }

        public abstract IPhase NextPhase();

        public virtual void Start() => StartTime = DateTime.Now;

        public virtual void Pause()
        {
            Elapsed += (DateTime.Now - StartTime).TotalMilliseconds;
            ChatManager.Pause();
        }

        public virtual double Resume()
        {
            StartTime = DateTime.Now;
            ChatManager.Resume();
            return Math.Max(0, Duration - Elapsed);
        }

        public virtual void End() => ChatManager.Close();
    }
}
