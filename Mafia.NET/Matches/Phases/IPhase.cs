using System;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases;

public interface IPhase
{
    IMatch Match { get; }
    Key Name { get; }
    double Duration { get; }
    DateTime StartTime { get; }
    double Elapsed { get; }
    IPhase? Supersedes { get; set; }
    IPhase? SupersededBy { get; set; }
    bool Skippable { get; }
    ChatManager ChatManager { get; }
    bool Actionable { get; }

    IPhase? NextPhase();
    void Start();
    void Pause();
    double Resume();
    void End();
}