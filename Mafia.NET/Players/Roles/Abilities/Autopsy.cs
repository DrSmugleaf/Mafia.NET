using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum AutopsyKey
    {
        StillAlive,
        AutopsyRole,
        AutopsyLastWill,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Autopsy", 9, typeof(AutopsySetup))]
    public class Autopsy : NightEndAbility<AutopsySetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<AutopsyKey>(abilities, TargetFilter.Dead(Match));
        }

        public override bool Use(IPlayer target)
        {
            if (target.Alive)
            {
                var notification = Notification.Chat(Role, AutopsyKey.StillAlive);
                User.OnNotification(notification);
                return false;
            }

            var message = new EntryBundle();
            message.Chat(Role, AutopsyKey.AutopsyRole, target, target.Role);
            if (Setup.DiscoverLastWill && target.LastWill.Text.Length > 0)
                message.Chat(Role, AutopsyKey.AutopsyLastWill, target.LastWill);

            User.OnNotification(message);

            return true;
        }
    }

    [RegisterSetup]
    public class AutopsySetup : IAbilitySetup
    {
        public bool DiscoverAllTargets { get; set; } = true; // TODO: Discover targets
        public bool DiscoverDeathType { get; set; } = true; // TODO: Death type
        public bool DiscoverLastWill { get; set; } = true;
    }
}