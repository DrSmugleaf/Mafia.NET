using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Neutral;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum DouseKey
    {
        ArsonistDouse,
        OtherDouse,
        UnDouse
    }

    public class Douse : AbilityAction<IDouseSetup>
    {
        public Douse(
            IAbility<IDouseSetup> ability,
            int priority = 5,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool TryUse()
        {
            if (!Filter(this) || !Ability.Active && !Setup.DousesRoleBlockers) return false;
            return ResolveUse();
        }

        public override bool Use()
        {
            User.Doused = false;
            var notification = Notification.Chat(Ability, DouseKey.UnDouse);
            User.OnNotification(notification);

            return true;
        }

        public override bool Use(IPlayer target)
        {
            if (target == User) return false;

            target.Doused = true;

            if (Setup.VictimNoticesDousing || target.Role.Ability is Arsonist)
            {
                var notification = target.Role.Ability.AbilitySetup is IDouseSetup
                    ? Notification.Chat(Ability, DouseKey.ArsonistDouse)
                    : Notification.Chat(Ability, DouseKey.OtherDouse);

                target.OnNotification(notification);
            }

            return true;
        }
    }

    public interface IDouseSetup : IAbilitySetup
    {
        public bool DousesRoleBlockers { get; set; }
        public bool IgnitionAlwaysKills { get; set; }
        public bool IgnitionKillsVictimsTargets { get; set; }
        public bool VictimNoticesDousing { get; set; }
    }
}