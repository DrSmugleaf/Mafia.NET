using Mafia.NET.Matches.Chats;
using System.Timers;

#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        IMatch Match { get; }
        string Name { get; }
        int Duration { get; }
        IPhase? Supersedes { get; set; }
        IPhase? SupersededBy { get; set; }
        bool Skippable { get; }
        ChatManager ChatManager { get; }
        Timer Timer { get; }

        IPhase NextPhase();
        void Start();
        void End();
    }
}
