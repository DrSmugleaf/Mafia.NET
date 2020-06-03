using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Alert : SelfImmunity
    {
        public Alert(
            IAbility ability,
            int strength = 2,
            int priority = 5,
            bool direct = true,
            bool stoppable = true) :
            base(ability, strength, priority, direct, stoppable)
        {
            Strength = strength;
        }

        public Alert(
            IAbility ability,
            AttackStrength strength = AttackStrength.Pierce,
            int priority = 5,
            bool direct = true,
            bool stoppable = true) :
            this(ability, (int) strength, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (Uses == 0 || !base.Use(target)) return false;
            Ability.Uses--;

            foreach (var visitor in Match.LivingPlayers)
            {
                var ability = visitor.Role.Ability;
                if (!ability.TargetManager.Any(User) ||
                    visitor == User ||
                    ability is Lookout ||
                    ability is Amnesiac ||
                    ability is Coroner ||
                    ability is Janitor) continue;

                var attack = new Attack(Ability, Strength);
                attack.Use(visitor);
            }

            return true;
        }
    }
}