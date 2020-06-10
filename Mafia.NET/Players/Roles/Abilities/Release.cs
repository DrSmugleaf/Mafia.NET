using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Release", -2)]
    public class Release : Ability
    {
        public override bool CanUse(IPlayer target)
        {
            return base.CanUse() &&
                   Targets.TryDay(out _) &&
                   !Targets.TryNight(out _);
        }

        public override PhaseTargeting ResolveTargets()
        {
            return Targets.GetAll(Time.Day);
        }

        public override bool Use(IPlayer prisoner)
        {
            prisoner.Perks.RoleBlock(User, true);
            return true;
        }
    }
}