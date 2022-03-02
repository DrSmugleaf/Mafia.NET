using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Execute", 5)]
    public class Execute : NightEndAbility
    {
        public Execute()
        {
            Strength = AttackStrength.Pierce;
            Filter = ability =>
                ability.Targets.Try(out var target) &&
                ability.Uses > 0 &&
                ability.Targets.TryDay(out var prisoner) &&
                target == prisoner;
        }

        public Detain Detain { get; set; } = null!;
        public AttackStrength Strength { get; set; }

        public override bool Active()
        {
            return base.Active() && Uses > 0;
        }

        public override bool Use(IPlayer target)
        {
            var attack = Attack(Strength, Priority);
            attack.Use(target);

            Uses--;
            Detain.Uses--;

            return true;
        }
    }
}