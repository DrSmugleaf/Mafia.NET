using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum BlackmailKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Blackmail", 4, typeof(BlackmailSetup))]
    public class Blackmail : NightEndAbility<BlackmailSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<BlackmailKey>(abilities, TargetFilter.Living(Match).Except(User.Role.Team));
        }

        public override bool Use(IPlayer target)
        {
            target.Perks.Blackmailed = true;
            return true;
        }
    }

    public class BlackmailSetup : IAbilitySetup
    {
        public bool BlackmailedTalkDuringTrial { get; set; }
    }
}