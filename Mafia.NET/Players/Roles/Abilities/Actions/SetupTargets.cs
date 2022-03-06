using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities.Actions;

public class SetupTargets : NightStartAbility
{
    public SetupTargets(
        IAbility parent,
        TargetFilter filter,
        TargetNotification notification,
        int priority = 0)
    {
        Parent = parent;
        Priority = priority;
        TargetFilter = filter;
        Notification = notification;
    }

    public IAbility Parent { get; set; }
    public TargetFilter TargetFilter { get; set; }
    public TargetNotification Notification { get; set; }

    public override bool Use()
    {
        User.Targets.Add(TargetFilter.Build(Parent, Notification));
        return true;
    }
}