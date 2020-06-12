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
    public enum HealKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetAttacked
    }

    [RegisterAbility("Heal", 7, typeof(HealSetup))]
    public class Heal : NightEndAbility<HealSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<HealKey>(abilities, TargetFilter.Living(Match).Except(User));
        }

        public override bool Use(IPlayer target)
        {
            if (target.Role.HealProfile.HealedBy(User) && Setup.KnowsIfTargetAttacked)
            {
                var notification = Notification.Chat(Role, HealKey.TargetAttacked);
                User.OnNotification(notification);
            }

            return true;
        }
    }

    [RegisterSetup]
    public class HealSetup : IAbilitySetup
    {
        public bool KnowsIfTargetConverted = false;
        public bool PreventsCultistConversion = false; // TODO
        public bool WitchDoctorWhenConverted = false;
        public bool KnowsIfTargetAttacked { get; set; } = true;
    }
}