using System;
using JetBrains.Annotations;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        IMatch Match { get; }
        string Name { get; }
        double Duration { get; }
        DateTime StartTime { get; }
        double Elapsed { get; }
        [CanBeNull] IPhase Supersedes { get; set; }
        [CanBeNull] IPhase SupersededBy { get; set; }
        bool Skippable { get; }
        ChatManager ChatManager { get; }
        bool Actionable { get; }

        IPhase NextPhase();
        void Start();
        void Pause();
        double Resume();
        void End();
    }
}