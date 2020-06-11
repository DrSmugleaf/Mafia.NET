using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Attack : Ability
    {
        public Attack(
            AttackStrength strength = AttackStrength.Base,
            int priority = 5,
            bool direct = true,
            bool stoppable = true)
        {
            Strength = strength;
            Priority = priority;
            Direct = direct;
            Stoppable = stoppable;
        }

        public Attack() : this(AttackStrength.Base)
        {
        }

        public AttackStrength Strength { get; set; }
        public bool Direct { get; set; }
        public bool Stoppable { get; set; }

        public override bool Use(IPlayer victim)
        {
            var threat = new Death(this, victim, Strength, Direct, Stoppable);
            User.Crimes.Add(CrimeKey.Murder);
            Match.Graveyard.Threats.Add(threat);
            return true;
        }
    }
}