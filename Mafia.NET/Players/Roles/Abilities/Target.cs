﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class Target
    {
        public Target(IAbility ability, TargetFilter filter, [CanBeNull] TargetNotification message = null)
        {
            Ability = ability;
            Filter = filter;
            Message = message ?? TargetNotification.Empty;
        }

        public IAbility Ability { get; }
        [CanBeNull] private IPlayer _targeted { get; set; }

        [CanBeNull]
        public IPlayer Targeted
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
                    Ability.User.OnNotification(userNotification);

                    var targetNotification = Message.TargetRemove(old);
                    old?.OnNotification(targetNotification);
                }
                else if (Targeted != null && old == null)
                {
                    var userNotification = Message.UserAdd(Targeted);
                    Ability.User.OnNotification(userNotification);

                    var targetNotification = Message.TargetAdd(Targeted);
                    Targeted?.OnNotification(targetNotification);
                }
                else if (Targeted != null && old != null)
                {
                    var userNotification = Message.UserChange(old, Targeted);
                    Ability.User.OnNotification(userNotification);

                    var targetNotification = Message.TargetChange(old, Targeted);
                    old?.OnNotification(targetNotification);
                    Targeted?.OnNotification(targetNotification);
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

        public void Set([CanBeNull] IPlayer target)
        {
            Targeted = target;
        }

        public void ForceSet([CanBeNull] IPlayer target)
        {
            _targeted = target;
        }

        public IReadOnlyList<IPlayer> ValidTargets(IMatch match)
        {
            return Filter.Filter(match.AllPlayers);
        }
    }
}