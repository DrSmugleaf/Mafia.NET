using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Targeting;

public class TargetFilter
{
    public static readonly TargetFilter Any = new(players => players);

    private readonly Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> _filter;

    private TargetFilter(Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> filter)
    {
        _filter = filter;
    }

    private TargetFilter(Func<IReadOnlyList<IPlayer>> supplier) : this(_ => supplier.Invoke())
    {
    }

    public static implicit operator TargetFilter(Func<IReadOnlyList<IPlayer>, IReadOnlyList<IPlayer>> filter)
    {
        return new TargetFilter(filter);
    }

    public static TargetFilter Living(IMatch match)
    {
        return new TargetFilter(() => match.LivingPlayers);
    }

    public static TargetFilter Dead(IMatch match)
    {
        return new TargetFilter(() => match.AllPlayers.Where(player => !player.Alive).ToList());
    }

    public static TargetFilter Only(IPlayer? player)
    {
        if (player == null) return None();

        return new TargetFilter(() => new List<IPlayer> {player});
    }

    public static TargetFilter None()
    {
        return new TargetFilter(ImmutableList.Create<IPlayer>);
    }

    public static TargetFilter Of(IReadOnlyList<IPlayer> players)
    {
        if (players.Count == 1) return Only(players.First());

        return new TargetFilter(players.ToImmutableList);
    }

    public IReadOnlyList<IPlayer> Filter(IReadOnlyList<IPlayer> players)
    {
        return _filter.Invoke(players);
    }

    public bool Valid(IPlayer target)
    {
        return _filter.Invoke(new List<IPlayer> {target}).Contains(target);
    }

    public TargetFilter Except(IPlayer player)
    {
        return new TargetFilter(players =>
        {
            players = Filter(players);
            return players.Where(entry => entry != player).ToList();
        });
    }

    public TargetFilter Except(ITeam team)
    {
        return new TargetFilter(players =>
        {
            players = Filter(players);
            return players.Where(entry => entry.Role.Team != team).ToList();
        });
    }

    public TargetFilter Except(TargetManager manager)
    {
        return new TargetFilter(players =>
        {
            players = Filter(players);
            return players
                .Where(player => !manager.Any(player))
                .ToList();
        });
    }

    public TargetFilter And(TargetFilter filter)
    {
        return new TargetFilter(players => filter.Filter(Filter(players)));
    }

    public TargetFilter Where(Func<IPlayer, bool> filter)
    {
        return new TargetFilter(players => _filter
            .Invoke(players)
            .Where(filter)
            .ToList());
    }

    public Target Build(IAbility ability, TargetNotification? message)
    {
        return new Target(ability, this, message);
    }
}