using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum InvestigateKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        ExactDetect,
        Detect
    }

    [RegisterAbility("Investigate", 9, typeof(InvestigateSetup))]
    public class Investigate : NightEndAbility<InvestigateSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<InvestigateKey>(abilities, TargetFilter.Living(Match).Except(User));
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            // TODO: Target switch
            var message = Setup.DetectsExactRole
                ? Notification.Chat(Role, InvestigateKey.ExactDetect, target, target.Crimes.RoleName())
                : target.Crimes.Crime(Role, InvestigateKey.Detect);

            User.OnNotification(message);

            return true;
        }
    }

    public class InvestigateSetup : IAbilitySetup
    {
        public bool DetectsExactRole { get; set; } = false;
    }
}