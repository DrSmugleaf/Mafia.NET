﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Targeting;

public class Target
{
    private IPlayer? _targeted;

    public Target(IAbility ability, TargetFilter filter, TargetNotification? message = null)
    {
        Ability = ability;
        Filter = filter;
        Message = message ?? TargetNotification.Empty;
    }

    public IAbility Ability { get; }
    public IPlayer User => Ability.User;

    public IPlayer? Targeted
    {
        get => _targeted;
        set
        {
            if (value != null && !Filter.Valid(value)) return;

            var old = _targeted;
            _targeted = value;

            if (Targeted == null && old != null)
            {
                var userNotification = Message.UserRemove(old);
                User.OnNotification(userNotification);

                var targetNotification = Message.TargetRemove(User, old);
                old.OnNotification(targetNotification);

                var teamNotification = Message.TeamRemove(User, old);
                foreach (var player in User.Match.AllPlayers)
                {
                    if (player == User || !Ability.IsTeammate(player)) continue;
                    player.OnNotification(teamNotification);
                }
            }
            else if (Targeted != null && old == null)
            {
                var userNotification = Message.UserAdd(Targeted);
                User.OnNotification(userNotification);

                var targetNotification = Message.TargetAdd(User, Targeted);
                Targeted?.OnNotification(targetNotification);

                var teamNotification = Message.TeamAdd(User, old);
                foreach (var player in User.Match.AllPlayers)
                {
                    if (player == User || !Ability.IsTeammate(player)) continue;
                    player.OnNotification(teamNotification);
                }
            }
            else if (Targeted != null && old != null)
            {
                var userNotification = Message.UserChange(old, Targeted);
                User.OnNotification(userNotification);

                var targetNotification = Message.TargetChange(User, old, Targeted);
                old.OnNotification(targetNotification);
                Targeted?.OnNotification(targetNotification);

                var teamNotification = Message.TeamChange(User, old, Targeted!);
                foreach (var player in User.Match.AllPlayers)
                {
                    if (player == User || !Ability.IsTeammate(player)) continue;
                    player.OnNotification(teamNotification);
                }
            }
        }
    }

    public TargetFilter Filter { get; }
    public TargetNotification Message { get; set; }

    public bool Try([MaybeNullWhen(false)] out IPlayer target)
    {
        target = Targeted;
        return target != null;
    }

    public void Set(IPlayer? target)
    {
        Targeted = target;
    }

    public void ForceSet(IPlayer? target)
    {
        _targeted = target;
    }

    public IReadOnlyList<IPlayer> ValidTargets(IMatch match)
    {
        return Filter.Filter(match.AllPlayers);
    }
}