using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class MafiaSuggest : AbilityAction<IMafiaSuggester>
    {
        public MafiaSuggest(
            IAbility<IMafiaSuggester> ability,
            int strength = 1,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            base(ability, priority, direct, stoppable)
        {
            Strength = strength;
        }

        public MafiaSuggest(
            IAbility<IMafiaSuggester> ability,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true,
            int priority = 5) :
            this(ability, (int) strength, direct, stoppable, priority)
        {
        }

        public int Strength { get; set; }

        public override bool Use(IPlayer victim)
        {
            var alliedSuggesterAttacked = Match.Graveyard
                .ThreatsOn(victim)
                .Any(threat =>
                    threat.Killer?.Role.Team == User.Role.Team &&
                    threat.Killer?.Ability.AbilitySetup is IMafiaSuggester);

            if (alliedSuggesterAttacked) return false;

            User.Crimes.Add(CrimeKey.Trespassing);
            var attack = new Attack(Ability, Strength, Direct, Stoppable, Priority);
            attack.Use(victim);

            return true;
        }
    }

    public interface IMafiaSuggester : IAbilitySetup
    {
    }
}