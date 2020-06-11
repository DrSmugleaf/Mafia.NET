using System.Collections.Generic;
using System.Collections.Immutable;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum AlertKey
    {
        UserAddMessage,
        UserRemoveMessage
    }

    [RegisterAbility("Alert", 5, typeof(AlertSetup))]
    public class Alert : NightEndAbility<AlertSetup>
    {
        // TODO: Early immunity
        private static readonly ImmutableArray<string> ImmuneRoles =
            ImmutableArray.Create("Lookout", "Amnesiac", "Coroner", "Janitor", "Incense Master");

        public AttackStrength Strength { get; set; } = AttackStrength.Pierce;

        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<AlertKey>(abilities, User);
        }

        public override bool Active()
        {
            return base.Active() && Uses > 0;
        }

        public override bool Use(IPlayer target)
        {
            if (target != User || Uses == 0) return false;

            Uses--;
            target.Perks.CurrentDefense = AttackStrength.Base;

            foreach (var visitor in Match.LivingPlayers)
            {
                // TODO: Targets vs visits (witch 1st vs 2nd)
                var role = visitor.Role.Id;
                if (!visitor.Targets.Any(User) ||
                    visitor == User ||
                    ImmuneRoles.Contains(role)) continue;

                var attack = Attack(Strength);
                attack.Use(visitor);
            }

            return true;
        }
    }

    public class AlertSetup : IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}