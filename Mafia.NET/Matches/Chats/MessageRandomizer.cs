using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mafia.NET.Matches.Chats
{
    public class MessageRandomizer
    {
        private static readonly Random Random = new Random();

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

        public string Get()
        {
            return Messages[Random.Next(Messages.Count)];
        }
    }
}