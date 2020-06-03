using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum BodyguardKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Bodyguard", typeof(BodyguardSetup))]
    public class Bodyguard : TownAbility<BodyguardSetup>
    {
        // TODO The Bodyguard will stay together with his guarded target. That means he won't die if a Mass Murderer visits his target, if that target visited someone else that night.
        public override bool HealedBy(IPlayer healer)
        {
            if (Setup.CanBeHealed) return base.HealedBy(healer);
            return Match.Graveyard.ThreatsOn(User).Count > 0;
        }

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var guard = new Guard(this);
            actions.Add(guard);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<BodyguardKey>());
        }
    }

    public class BodyguardSetup : ITownSetup, IGuardSetup
    {
        public bool CanBeHealed = false;
        public bool PreventsCultistConversion = false; // TODO: Prevents conversions
        public bool IgnoresInvulnerability { get; set; } = true;
    }
}