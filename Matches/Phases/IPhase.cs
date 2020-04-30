#nullable enable

namespace Mafia.NET.Matches.Phases
{
    public interface IPhase
    {
        string Name { get; }
        IPhase ?Supersedes { get; set; }
        IPhase ?SupersededBy { get; set; }
        IPhase ?NextPhase { get; }
        bool Skippable { get; }
        void Start();
        void End();
    }
}
