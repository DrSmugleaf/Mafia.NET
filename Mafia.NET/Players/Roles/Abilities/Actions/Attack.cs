using Mafia.NET.Players.Deaths;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Attack : AbilityAction
    {
        public Attack(
            IAbility ability,
            int strength = 1,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            base(ability, priority, direct, stoppable)
        {
            Strength = strength;
        }

        public Attack(
            IAbility ability,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            this(ability, (int) strength, direct, stoppable, priority)
        {
        }

        public int Strength { get; set; }

        public bool VictimVulnerable(IPlayer victim)
        {
            return victim.Ability.VulnerableTo(Strength);
        }

        public override bool Use(IPlayer victim)
        {
            if (!VictimVulnerable(victim)) return false;

            var threat = new Death(Ability, victim, Direct, Stoppable);
            User.Crimes.Add(CrimeKey.Murder);
            Match.Graveyard.Threats.Add(threat);
            return true;
        }
    }
}