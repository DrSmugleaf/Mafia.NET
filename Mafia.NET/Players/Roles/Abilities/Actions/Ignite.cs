namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Ignite : AbilityAction<IIgniteSetup>
    {
        public Ignite(
            IAbility<IIgniteSetup> ability,
            int priority = 5,
            bool direct = false,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            if (target != User) return false;

            foreach (var player in Match.LivingPlayers)
            {
                if (!player.Doused) continue;

                var stoppable = !Setup.IgnitionAlwaysKills;
                var strength = Setup.IgnitionAlwaysKills
                    ? AttackStrength.Pierce
                    : AttackStrength.Base;

                var attack = new Attack(Ability, strength, Direct, stoppable);
                attack.Use(player);

                if (Setup.IgnitionKillsVictimsTargets &&
                    player.TargetManager.Try(out var victimsTarget))
                    attack.Use(victimsTarget);
            }

            return true;
        }
    }

    public interface IIgniteSetup : IAbilitySetup
    {
        public bool IgnitionAlwaysKills { get; set; }
        public bool IgnitionKillsVictimsTargets { get; set; }
    }
}