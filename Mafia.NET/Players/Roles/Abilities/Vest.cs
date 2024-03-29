﻿using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum VestKey
{
    UsesLeft,
    UsesLeftPlural,
    UsedUp,
    UsedUpNow,
    UserAddMessage,
    UserRemoveMessage
}

[RegisterAbility("Vest", 1, typeof(VestSetup))]
public class Vest : NightEndAbility<VestSetup>
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        SetupTargets<VestKey>(abilities, User);

        if (Uses == 0)
        {
            User.OnNotification(Notification.Chat(Role, VestKey.UsedUp));
            return;
        }

        var usesLeft = Notification.Chat(Role, Uses == 1 ? VestKey.UsesLeft : VestKey.UsesLeftPlural, Uses);
        User.OnNotification(usesLeft);
    }

    public override bool Use(IPlayer target)
    {
        if (target != User || Uses == 0) return false;

        Uses--;
        target.Perks.CurrentDefense = AttackStrength.Base;

        if (Uses == 0)
        {
            var notification = Notification.Chat(Role, VestKey.UsedUpNow);
            User.OnNotification(notification);
        }

        return true;
    }
}

[RegisterSetup]
public class VestSetup : IAbilitySetup
{
    public bool WinTiesAgainstMafia = true; // TODO 1:1s and tiebreakers
}