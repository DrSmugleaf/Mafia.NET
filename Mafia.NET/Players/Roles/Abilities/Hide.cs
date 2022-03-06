using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum HideKey
{
    SomeoneHide,
    SelfHide,
    HideAt
}

[RegisterAbility("Hide", 2, typeof(HideSetup))]
public class Hide : NightEndAbility<HideSetup>
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        if (Uses == 0) return;

        var filter = Setup.CanHideBehindMafia
            ? TargetFilter.Living(Match).Except(User)
            : TargetFilter.Living(Match).Except(User.Role.Team);

        SetupTargets<HideKey>(abilities, filter);
    }

    public override bool Use(IPlayer target)
    {
        if (Uses == 0) return false;

        User.Crimes.Add(CrimeKey.Trespassing);

        foreach (var player in Match.LivingPlayers)
        {
            var targets = player.Targets;
            if (targets[0] == User) targets.ForceSet(target);
        }

        if (Setup.NotifiesTarget)
            target.OnNotification(Notification.Chat(Role, HideKey.SomeoneHide));

        var notification = target == User
            ? Notification.Chat(Role, HideKey.SelfHide)
            : Notification.Chat(Role, HideKey.HideAt, target); // TODO: Attribute kills to the Beguiler

        User.OnNotification(notification);

        Uses--;

        return true;
    }
}

[RegisterSetup]
public class HideSetup : IAbilitySetup
{
    public bool CanHideBehindMafia = false;
    public bool NotifiesTarget { get; set; }
}