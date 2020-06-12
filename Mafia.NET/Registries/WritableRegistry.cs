using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Registries
{
    public abstract class WritableRegistry<T, TId> where T : IRegistrable<TId>
    {
        protected WritableRegistry(Dictionary<TId, T> ids)
        {
            Ids = new ConcurrentDictionary<TId, T>(ids);
        }

        protected WritableRegistry()
        {
            Ids = new ConcurrentDictionary<TId, T>();
        }

        protected ConcurrentDictionary<TId, T> Ids { get; }

        public T this[TId id]
        {
            // ReSharper disable once HeapView.CanAvoidClosure
            get => Ids.GetOrAdd(id, newId => Register(Create(newId), true));
            set => Register(value, true);
        }

        public virtual T Register(T entry, bool ignore = false)
        {
            var id = entry.Id;
            if (!ignore && Ids.ContainsKey(id))
                throw new ArgumentException($"Key with id {id} already exists.");

            Ids[id] = entry;
            return entry;
        }

        public List<T> All()
        {
            return Ids.Values.ToList();
        }

        public List<T> Get(params TId[] ids)
        {
            var entries = new List<T>();

            foreach (var id in ids)
            {
                var entry = this[id];
                entries.Add(entry);
            }

            return entries;
        }

        public abstract T Create(TId id);
    }

    public abstract class WritableRegistry<T> : WritableRegistry<T, string>
        where T : IRegistrable<string>
    {
    }
}