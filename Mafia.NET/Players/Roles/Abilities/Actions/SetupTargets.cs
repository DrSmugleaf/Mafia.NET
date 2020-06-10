using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class SetupTargets : NightStartAbility
    {
        public SetupTargets(
            TargetFilter filter,
            TargetNotification notification,
            int priority = 0)
        {
            Priority = priority;
            TargetFilter = filter;
            Notification = notification;
        }

        public TargetFilter TargetFilter { get; set; }
        public TargetNotification Notification { get; set; }

        public override bool Use()
        {
            User.Targets.Add(TargetFilter.Build(User, Notification));
            return true;
        }
    }
}