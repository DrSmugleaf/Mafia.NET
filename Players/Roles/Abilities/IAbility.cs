using Mafia.NET.Matches;
using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; }
        IPlayer User { get; }
        string Name { get; }
        AbilityPhase Phase { get; }

        IDictionary<int, IPlayer> ValidTargets();
        void OnDayStart();
        void OnDayEnd();
        void OnNightStart();
        void OnNightEnd();
    }

    public interface IAbility<T> : IAbility where T : ITarget
    {
        bool UsableDay(T target);
        bool UsableNight(T target);
        bool TryUse(T target);
        void UseDay(T target);
        void UseNight(T target);
        bool CanTarget(IPlayer player);
        bool CanTarget(T target);
    }
}
