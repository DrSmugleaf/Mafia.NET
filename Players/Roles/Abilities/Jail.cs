using Mafia.NET.Matches;
using Mafia.NET.Players.Deaths;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Jail", typeof(JailSetup))]
    public class Jail : BaseAbility
    {
        protected JailSetup Setup { get; set; }

        public Jail(IMatch match, IPlayer user) : base(match, user, "Jail", AbilityPhase.BOTH)
        {
            Setup = (JailSetup)match.Setup.Roles.Abilities[Name];
        }

        protected override void OnDayStart()
        {
            Targeting.Get().Targets = new List<Target>()
            {
                TargetFilter.Living(Match).Except(User)
            };
        }

        protected override void OnDayEnd()
        {
            if (Match.Graveyard.Any(death => death.Day == Match.Day && death.Cause == DeathCause.LYNCH)) {
                Targeting.Get().Targets[0].Targeted = null;
            }
        }

        protected override void OnNightStart()
        {
            var prisoner = Targeting.Phases[TimePhase.DAY].Targets[0].Targeted;
            if (prisoner == null) return;

            Target target = Setup.Executions > 0 ? TargetFilter.Only(prisoner) : TargetFilter.None();
            Targeting.Get().Targets = new List<Target>() { target };
        }

        protected override void OnNightEnd()
        {
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions { get; set; } = 1;

        public JailSetup()
        {
        }
    }
}
