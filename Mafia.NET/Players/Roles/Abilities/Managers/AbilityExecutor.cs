using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Roles.Abilities.Managers;

public class AbilityExecutor
{
    public AbilityExecutor(IMatch match)
    {
        Match = match;
    }

    public IMatch Match { get; }

    public IEnumerable<AbilityManager> Abilities()
    {
        return Match.AllPlayers.Select(player => player.Abilities);
    }

    public IEnumerable<IAbility> Order(Func<AbilityManager, IList<IAbility>> supplier)
    {
        return Abilities()
            .SelectMany(supplier)
            .OrderBy(ability => ability.Priority)
            .ThenBy(ability => ability.User.Number);
    }

    public void OnDayStart()
    {
        var abilities = Order(manager => manager.DayStart());
        foreach (var ability in abilities) ability.ResolveUse();
    }

    public void OnDayEnd()
    {
        var abilities = Order(manager => manager.DayEnd());
        foreach (var ability in abilities) ability.ResolveUse();
    }

    public void OnNightStart()
    {
        var abilities = Order(manager => manager.NightStart());
        foreach (var ability in abilities) ability.ResolveUse();
    }

    public void OnNightEnd()
    {
        var abilities = Order(manager => manager.NightEnd());
        foreach (var ability in abilities) ability.ResolveUse();
    }
}