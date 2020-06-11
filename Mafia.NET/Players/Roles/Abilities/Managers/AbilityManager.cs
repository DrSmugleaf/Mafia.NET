using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;

namespace Mafia.NET.Players.Roles.Abilities.Managers
{
    public class AbilityManager
    {
        public AbilityManager([CanBeNull] IEnumerable<IAbility> abilities = null)
        {
            All = abilities?.ToList() ?? new List<IAbility>();
        }

        public IList<IAbility> All { get; set; }

        public void Replace(AbilityRegistry registry, IEnumerable<string> ids, IPlayer user)
        {
            All = registry.Abilities(user, ids);
        }

        public bool Any(params Type[] types)
        {
            return All.Any(ability => types.Contains(ability.GetType()));
        }

        public bool Any<T>() where T : IAbility
        {
            return Any(typeof(T));
        }

        [CanBeNull]
        public IAbility Get(Type type)
        {
            return All.FirstOrDefault(ability => ability.Is(type));
        }

        [CanBeNull]
        public T Get<T>() where T : IAbility
        {
            return (T) Get(typeof(T));
        }

        public IList<IAbility> DayStart()
        {
            var dayStart = new List<IAbility>();
            foreach (var ability in All) ability.DayStart(dayStart);
            return dayStart;
        }

        public IList<IAbility> DayEnd()
        {
            var dayEnd = new List<IAbility>();
            foreach (var ability in All) ability.DayEnd(dayEnd);
            return dayEnd;
        }

        public IList<IAbility> NightStart()
        {
            var nightStart = new List<IAbility>();
            foreach (var ability in All) ability.NightStart(nightStart);
            return nightStart;
        }

        public IList<IAbility> NightEnd()
        {
            var nightEnd = new List<IAbility>();
            foreach (var ability in All) ability.NightEnd(nightEnd);
            return nightEnd;
        }
    }
}