using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum AnnoyKey
{
    UserAddMessage,
    UserRemoveMessage,
    UserChangeMessage,
    UserAnnoy,
    Annoyed,
    AnnoyedShoot
}

[RegisterAbility("Annoy", 5)]
public class Annoy : NightEndAbility
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        var filter = TargetFilter.Living(Match).Except(User);
        SetupTargets<AnnoyKey>(abilities, filter);
    }

    public override bool Use(IPlayer target)
    {
        var userNotification = Notification.Chat(Role, AnnoyKey.UserAnnoy, target);
        User.OnNotification(userNotification);

        var isVeteran = target.Abilities.Any<Alert>() && target.Targets.Any(target);
        var annoy = isVeteran
            ? Notification.Chat(Role, AnnoyKey.AnnoyedShoot)
            : Notification.Chat(Role, AnnoyKey.Annoyed);
        target.OnNotification(annoy);

        return true;
    }
}