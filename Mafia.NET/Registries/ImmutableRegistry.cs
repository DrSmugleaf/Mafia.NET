using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Mafia.NET.Registries
{
    public abstract class ImmutableRegistry<T> where T : IRegistrable
    {
        protected ImmutableRegistry(Dictionary<string, T> ids)
        {
            Ids = ids.ToImmutableDictionary();
        }

        public ImmutableDictionary<string, T> Ids { get; }

        public virtual T this[string id] => Ids[id];

        public List<T> Entries()
        {
            return Ids.Values.ToList();
        }

        public List<T> Get(params string[] ids)
        {
            var entries = new List<T>();

            foreach (var id in ids)
            {
                var entry = this[id];
                entries.Add(entry);
            }

            return entries;
        }
    }
}