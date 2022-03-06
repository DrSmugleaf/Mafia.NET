using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mafia.NET.Matches.Chats;

public class MessageRandomizer
{
    public MessageRandomizer(IEnumerable<string> messages)
    {
        Messages = messages.ToImmutableList();
    }

    public MessageRandomizer(params string[] messages)
    {
        Messages = messages.ToImmutableList();
    }

    public MessageRandomizer()
    {
        Messages = ImmutableList.Create<string>();
    }

    public IImmutableList<string> Messages { get; }

    public string Get(Random random)
    {
        return Messages[random.Next(Messages.Count)];
    }
}