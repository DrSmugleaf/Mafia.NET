using Mafia.NET.Matches.Chats;
using System;

#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        IMatch Match { get; }
        string Name { get; }
        DateTime StartTime { get; }
        double Duration { get; }
        IPhase? Supersedes { get; set; }
        IPhase? SupersededBy { get; set; }
        bool Skippable { get; }
        ChatManager ChatManager { get; }

        IPhase NextPhase();
        void Start();
        void Pause();
        void Resume();
        void End();
    }
}
