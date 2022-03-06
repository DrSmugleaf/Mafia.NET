using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Chats;

public interface IChat
{
    string Id { get; set; }
    bool Paused { get; set; }
    IDictionary<IPlayer, IChatParticipant> Participants { get; set; }

    void Initialize(IMatch match);
    IChatParticipant Add(IPlayer player, bool muted = false, bool deaf = false);
    IChat Add(IEnumerable<IPlayer> players, bool muted = false, bool deaf = false);
    IChatParticipant Get(IPlayer player);
    IChat Mute(IPlayer player, bool muted = true);
    IChat Mute(bool muted = true);
    IChat Deafen(IPlayer player, bool deaf = true);
    IChat Deafen(bool deaf = true);
    IChat Disable(IPlayer player, bool disabled = true);
    IChat Disable(bool disabled = true);
    IChat Pause(bool paused = true);
    bool CanSend(MessageIn messageIn);
    bool TrySend(MessageIn messageIn, [NotNullWhen(true)] out MessageOut? messageOut);
    bool TrySend(IPlayer player, string text, [NotNullWhen(true)] out MessageOut? messageOut);

    void Close();
}

public class Chat : IChat
{
    public Chat(string id, Dictionary<IPlayer, IChatParticipant> participants)
    {
        Id = id;
        Participants = participants;
    }

    public Chat(string id)
    {
        Id = id;
        Participants = new Dictionary<IPlayer, IChatParticipant>();
    }

    public Chat() : this(string.Empty)
    {
    }

    protected bool Initialized { get; set; }
    public string Id { get; set; }
    public bool Paused { get; set; }
    public IDictionary<IPlayer, IChatParticipant> Participants { get; set; }

    public virtual void Initialize(IMatch match)
    {
    }

    public IChatParticipant Add(IPlayer player, bool muted = false, bool deaf = false)
    {
        var participant = new ChatParticipant(player, muted, deaf);
        Participants[player] = participant;
        return participant;
    }

    public IChat Add(IEnumerable<IPlayer> players, bool muted = false, bool deaf = false)
    {
        foreach (var player in players)
            Add(player);

        return this;
    }

    public IChatParticipant Get(IPlayer player)
    {
        if (!Participants.TryGetValue(player, out var participant))
            participant = Add(player);

        return participant;
    }

    public IChat Mute(IPlayer player, bool muted = true)
    {
        Get(player).Muted = muted;
        return this;
    }

    public IChat Mute(bool muted = true)
    {
        foreach (var player in Participants.Keys)
            Mute(player, muted);

        return this;
    }

    public IChat Deafen(IPlayer player, bool deaf = true)
    {
        Get(player).Deaf = deaf;
        return this;
    }

    public IChat Deafen(bool deaf = true)
    {
        foreach (var player in Participants.Keys)
            Deafen(player, deaf);

        return this;
    }

    public IChat Disable(IPlayer player, bool disabled = true)
    {
        var participant = Get(player);
        participant.Muted = disabled;
        participant.Deaf = disabled;

        return this;
    }

    public IChat Disable(bool disabled = true)
    {
        foreach (var player in Participants.Keys)
            Disable(player, disabled);

        return this;
    }

    public IChat Pause(bool paused = true)
    {
        Paused = paused;
        return this;
    }

    public bool CanSend(MessageIn messageIn)
    {
        return !Paused &&
               Participants.ContainsKey(messageIn.Sender.Owner) &&
               messageIn.Sender.CanSend() &&
               messageIn.Text.Length > 0;
    }

    public bool TrySend(MessageIn messageIn, [NotNullWhen(true)] out MessageOut? messageOut)
    {
        messageOut = null;
        if (!CanSend(messageIn)) return false;

        var listeners = new HashSet<IPlayer>();

        foreach (var participant in Participants.Values)
            if (participant.CanReceive())
                listeners.Add(participant.Owner);

        messageOut = new MessageOut(messageIn, listeners);

        return true;
    }

    public bool TrySend(IPlayer player, string text, [NotNullWhen(true)] out MessageOut? messageOut)
    {
        messageOut = null;
        if (!Participants.TryGetValue(player, out var participant)) return false;

        var messageIn = new MessageIn(participant, text);
        return TrySend(messageIn, out messageOut);
    }

    public void Close()
    {
        Participants.Clear();
    }
}