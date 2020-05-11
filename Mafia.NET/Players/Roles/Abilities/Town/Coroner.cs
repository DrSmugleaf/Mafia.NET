﻿using System;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Coroner", typeof(CoronerSetup))]
    public class Coroner : TownAbility<CoronerSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            if (target.Alive)
            {
                User.OnNotification(Notification.Chat("Your target is still alive."));
                return;
            }

            var message = $"{target.Name}'s role was {target.Role}.";
            if (Setup.DiscoverLastWill && target.LastWill.Text.Length > 0)
                message += $"{Environment.NewLine}Their last will was:{Environment.NewLine}{target.LastWill}";

            var notification = Notification.Chat(message);
            User.OnNotification(notification);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Dead(Match), new TargetNotification
            {
                UserAddMessage = target => $"You will perform an autopsy on {target.Name}.",
                UserRemoveMessage = target => "You won't perform an autopsy tonight.",
                UserChangeMessage = (old, current) => $"You will instead perform an autopsy on {current.Name}."
            });
        }
    }

    public class CoronerSetup : ITownSetup
    {
        public bool DiscoverAllTargets = true; // TODO: Discover targets
        public bool DiscoverDeathType = true; // TODO: Death type
        public bool DiscoverLastWill = true;
    }
}