using Mafia.NET.Players.Deaths;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Jailor", typeof(JailSetup))]
    public class Jailor : BaseAbility<JailSetup>
    {
        protected override void _onDayStart()
        {
            TargetManager += TargetFilter.Living(Match).Except(User);
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.DiedToday(DeathCause.LYNCH)) TargetManager[0].Targeted = null;
        }

        protected override void _onNightStart()
        {
            var prisoner = TargetManager.Day()[0].Targeted;
            if (prisoner == null) return;
            TargetManager += Setup.Executions > 0 ? prisoner : null;
        }

        protected override void _onNightEnd()
        {
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions = 1;
    }
}
