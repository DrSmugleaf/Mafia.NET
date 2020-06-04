using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class MafiaHead : AbilityAction<IMafiaHead>
    {
        public MafiaHead(
            IAbility<IMafiaHead> ability,
            int priority = 4,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
        }

        public bool TryMinion([MaybeNullWhen(false)] out IPlayer minion)
        {
            minion = Match.LivingPlayers
                .FirstOrDefault(player =>
                    player.Role.Team == User.Role.Team &&
                    player.Ability.AbilitySetup is IMafiaSuggester);

            return minion != null;
        }

        public override bool Use(IPlayer target)
        {
            if (TryMinion(out var minion))
            {
                minion.TargetManager.ForceSet(target);
                TargetManager.ForceSet(null);

                return true;
            }

            if (Setup.CanKillWithoutMafioso)
            {
                var attack = new Attack(Ability, AttackStrength.Base, Direct, Stoppable, Priority);
                attack.Use(target);
            }

            return false;
        }
    }

    public interface IMafiaHead : IAbilitySetup
    {
        public bool CanKillWithoutMafioso { get; set; }
    }
}