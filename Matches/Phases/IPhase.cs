#nullable enable

using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        IMatch Match { get; }
        string Name { get; }
        int Duration { get; }
        sealed double DurationMs => Duration * 1000;
        IPhase? PreviousPhase { get; }
        IPhase? NextPhase { get; }
        IPhase? Supersedes { get; set; }
        IPhase? SupersededBy { get; set; }
        bool Skippable { get; }
        ChatManager ChatManager { get; }
        void Start();
        IPhase End();
    }
}
