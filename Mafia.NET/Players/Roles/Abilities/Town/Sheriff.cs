using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
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

    [RegisterAbility("Sheriff", typeof(SheriffSetup))]
    public class Sheriff : TownAbility<SheriffSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var sheriff = new SheriffAction(this);
            actions.Add(sheriff);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<SheriffKey>());
        }
    }

    public class SheriffSetup : ITownSetup, ISheriffSetup
    {
        public bool DetectsMafiaTriad { get; set; } = true;
        public bool DetectsSerialKiller { get; set; } = true;
        public bool DetectsArsonist { get; set; } = true;
        public bool DetectsCult { get; set; } = true;
        public bool DetectsMassMurderer { get; set; } = true;
    }
}