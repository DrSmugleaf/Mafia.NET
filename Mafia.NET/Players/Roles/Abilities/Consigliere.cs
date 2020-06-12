using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum ConsigliereKey
    {
        ExactDetect,
        Detect,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Consigliere", 9, typeof(ConsigliereSetup))]
    public class Consigliere : NightEndAbility<ConsigliereSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<ConsigliereKey>(abilities, TargetFilter.Living(Match).Except(User.Role.Team));
        }

        public override void NightEnd(in IList<IAbility> abilities)
        {
            var investigate = Get<Investigate>();
            abilities.Add(investigate);
        }
    }

    [RegisterSetup]
    public class ConsigliereSetup : InvestigateSetup
    {
        public ConsigliereSetup()
        {
            DetectsExactRole = false;
        }
    }
}