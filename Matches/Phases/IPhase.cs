#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        string Name { get; }
        int Duration { get; }
        IPhase? Supersedes { get; set; }
        IPhase? SupersededBy { get; set; }
        IPhase? PreviousPhase { get; }
        IPhase? NextPhase { get; }
        bool Skippable { get; }
        void Start(IMatch match);
        void End(IMatch match);
    }
}
