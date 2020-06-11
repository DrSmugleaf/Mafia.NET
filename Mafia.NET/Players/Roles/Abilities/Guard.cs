using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum GuardKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Guard", 7, typeof(GuardSetup))]
    public class Guard : NightEndAbility<GuardSetup>
    {
        // TODO The Bodyguard will stay together with his guarded target. That means he won't die if a Mass Murderer visits his target, if that target visited someone else that night.
        // TODO Manipulated to target self
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<GuardKey>(abilities, TargetFilter.Living(Match).Except(User));
        }

        public override void NightEnd(in IList<IAbility> abilities)
        {
            base.NightEnd(in abilities);

            var protect = Get<Protect>();
            abilities.Add(protect);
        }

        public override bool Use(IPlayer target)
        {
            var threats = Match.Graveyard.ThreatsOn(target)
                .Where(death => death.Direct)
                .ToList();

            if (threats.Count > 0)
            {
                var threat = threats[0];
                threat.WithVictim(User);

                var strength = AttackStrength.Base;
                if (Setup.IgnoresInvulnerability) strength = AttackStrength.Pierce;

                var killer = threat.Killer;
                if (killer == null) return true;

                var attack = Attack(strength, Priority);
                attack.Use(killer);

                return true;
            }

            return false;
        }
    }

    public class GuardSetup : IAbilitySetup
    {
        public bool PreventsCultistConversion = false; // TODO: Prevents conversions
        public bool IgnoresInvulnerability { get; set; } = true;
    }
}