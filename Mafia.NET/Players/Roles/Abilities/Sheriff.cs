using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum SheriffKey
    {
        NotSuspicious,
        Mafia,
        Triad,
        Cultist,
        Arsonist,
        MassMurderer,
        SerialKiller,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Sheriff", 9, typeof(SheriffSetup))]
    public class Sheriff : NightEndAbility<SheriffSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match).Except(User);
            SetupTargets<SheriffKey>(abilities, filter);
        }

        public override bool Use(IPlayer target)
        {
            var message = target.Crimes.Sheriff(Setup).Chat();
            User.OnNotification(message);

            return true;
        }
    }

    [RegisterSetup]
    public class SheriffSetup : IAbilitySetup
    {
        public bool DetectsMafiaTriad { get; set; } = true;
        public bool DetectsSerialKiller { get; set; } = true;
        public bool DetectsArsonist { get; set; } = true;
        public bool DetectsCult { get; set; } = true;
        public bool DetectsMassMurderer { get; set; } = true;
    }
}