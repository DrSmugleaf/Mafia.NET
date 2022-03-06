using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum AgentKey
{
    TargetInactive,
    TargetVisitedSomeone,
    SomeoneVisitedTarget,
    NoneVisitedTarget,
    UserAddMessage,
    UserRemoveMessage,
    UserChangeMessage
}

[RegisterAbility("Agent", 9, typeof(AgentSetup))]
public class Agent : NightEndAbility<AgentSetup>
{
    public override void NightStart(in IList<IAbility> abilities)
    {
        if (Cooldown > 0) return;
        SetupTargets<AgentKey>(abilities, TargetFilter.Living(Match));
    }

    public override bool Active()
    {
        return base.Active() && Cooldown == 0;
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
            Cooldown--;
            return false;
        }

        var detect = Get<Detect>();
        var watch = Get<Watch>();

        detect.Use(target);
        watch.Use(target);

        Cooldown = Setup.NightsBetweenUses;

        return true;
    }
}

[RegisterSetup]
public class AgentSetup : IWatchSetup, ICooldownSetup
{
    public int NightsBetweenUses { get; set; } = 1;
    public bool IgnoresDetectionImmunity { get; set; }
    public bool CanTargetSelf { get; set; } = true;
}