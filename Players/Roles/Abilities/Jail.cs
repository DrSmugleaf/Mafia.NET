using Mafia.NET.Matches;
using Mafia.NET.Players.Deaths;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Jail", typeof(JailSetup))]
    public class Jail : BaseAbility<SingleTarget>
    {
        public JailSetup Setup { get; protected set; }
        public bool Execute { get; protected set; }

        public Jail(IMatch match, IPlayer user) : base(match, user, "Jail", AbilityPhase.BOTH)
        {
            Setup = (JailSetup)match.Setup.Roles.Abilities[Name];
            Execute = false;
        }

        public override bool UsableDay(SingleTarget target)
        {
            return base.UsableDay(target) && !Match.Graveyard.Any(death => death.Day == Match.Day && death.Cause == DeathCause.LYNCH);
        }

        public override bool UsableNight(SingleTarget target) {
            return base.UsableNight(target) && Setup.Executions > 0;
        }

        public override void UseDay(SingleTarget target)
        {
            Target = target;
        }

        public override void UseNight(SingleTarget target)
        {
            Execute = !Execute;
        }

        public override void OnDayStart()
        {
            base.OnDayStart();
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
