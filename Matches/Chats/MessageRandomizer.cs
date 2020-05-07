using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Matches.Chats
{
    public class MessageRandomizer
    {
        private static Random Random { get; } = new Random();
        public IImmutableList<string> Messages { get; }

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

        public string Get()
        {
            return Messages[Random.Next(Messages.Count)];
        }
    }
}
