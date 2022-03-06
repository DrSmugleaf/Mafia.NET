using System;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases;

public abstract class BasePhase : IPhase
{
    protected BasePhase(IMatch match, string name, uint duration, bool skippable = false, bool actionable = true)
    {
        Match = match;
        Name = new Key($"phase{name}");
        Duration = duration * 1000;
        Skippable = skippable;
        Actionable = actionable; // TODO
    }

    public IMatch Match { get; }
    public Key Name { get; }
    public double Duration { get; protected set; }
    public DateTime StartTime { get; protected set; }
    public double Elapsed { get; protected set; }
    public IPhase? Supersedes { get; set; }
    public IPhase? SupersededBy { get; set; }
    public bool Skippable { get; }
    public ChatManager ChatManager => Match.Chat;
    public bool Actionable { get; }

    public abstract IPhase? NextPhase();

    public virtual void Start()
    {
        StartTime = DateTime.Now;
    }

    public virtual void Pause()
    {
        Elapsed += (DateTime.Now - StartTime).TotalMilliseconds;
        ChatManager.Main().Pause();
    }

    public virtual double Resume()
    {
        StartTime = DateTime.Now;
        ChatManager.Main().Pause(false);
        return Math.Max(0, Duration - Elapsed);
    }

    public virtual void End()
    {
    }
}