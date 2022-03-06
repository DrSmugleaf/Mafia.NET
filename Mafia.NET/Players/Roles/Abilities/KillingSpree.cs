using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum KillingSpreeKey
{
    UserAddMessage,
    UserRemoveMessage,
    UserChangeMessage,
    OnCooldown,
    OnCooldownPlural
}

[RegisterAbility("Killing Spree", 6, typeof(KillingSpreeSetup))]
public class KillingSpree : NightEndAbility<IKillingSpreeSetup>
{
    // TODO Kill depending on visit, including staying at ones own house
    public override void NightStart(in IList<IAbility> abilities)
    {
        if (Cooldown > 0) return;

        var filter = TargetFilter.Living(Match);
        if (!Setup.CanTargetSelf) filter = filter.Except(User);

        SetupTargets<KillingSpreeKey>(abilities, filter);
    }

    public override bool Use()
    {
        Cooldown--;
        return false;
    }

    public override bool Use(IPlayer target)
    {
        if (Cooldown > 0)
        {
            var onCooldown = Notification.Chat(Role, Uses == 1
                ? KillingSpreeKey.OnCooldown
                : KillingSpreeKey.OnCooldownPlural, Uses);
            User.OnNotification(onCooldown);

            Cooldown--;
            return false;
        }

        if (target == User && !Setup.CanTargetSelf) return false;

        var kills = 0;
        foreach (var player in Match.AllPlayers)
        {
            if (player == User) continue;

            if (player.Targets.Any(target))
            {
                var attack = Attack();
                attack.Use(player);
                kills++;
            }
        }

        if (kills > 1) Cooldown = Setup.NightsBetweenUses;

        return kills > 0;
    }
}

public interface IKillingSpreeSetup : ICooldownSetup
{
    bool CanTargetSelf { get; set; }
}

public class KillingSpreeSetup : IKillingSpreeSetup
{
    public int NightsBetweenUses { get; set; } = 1;
    public bool CanTargetSelf { get; set; } = true;
}