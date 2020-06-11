using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Registries
{
    public abstract class WritableRegistry<T> where T : IRegistrable
    {
        protected WritableRegistry(Dictionary<string, T> ids)
        {
            Ids = new ConcurrentDictionary<string, T>(ids);
        }

        protected WritableRegistry()
        {
            Ids = new ConcurrentDictionary<string, T>();
        }

        protected ConcurrentDictionary<string, T> Ids { get; }

        public virtual T this[string id]
        {
            get => Ids.GetOrAdd(id, Create);
            set => Register(value, true);
        }

        public void Register(T entry, bool ignore = false)
        {
            var id = entry.Id;
            if (!ignore && Ids.ContainsKey(id))
                throw new ArgumentException($"Key with id {id} already exists.");

            Ids[id] = entry;
        }

        public List<T> All()
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

        public abstract T Create(string id);
    }
}