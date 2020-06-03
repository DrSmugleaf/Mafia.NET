using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Guard : AbilityAction<IGuardSetup>
    {
        public Guard(
            IAbility<IGuardSetup> user,
            int priority = 7,
            bool direct = true,
            bool stoppable = true) :
            base(user, priority, direct, stoppable)
        {
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

                var attack = new Attack(Ability, strength, Direct, Stoppable, Priority);
                attack.Use(killer);

                return true;
            }

            return false;
        }
    }

    public interface IGuardSetup : IAbilitySetup
    {
        public bool IgnoresInvulnerability { get; set; }
    }
}