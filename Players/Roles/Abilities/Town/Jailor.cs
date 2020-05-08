namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Jailor", typeof(JailSetup))]
    public class Jailor : BaseAbility<JailSetup>
    {
        public int Executions { get; set; }

        public Jailor()
        {
            Executions = Setup.Executions;
        }

        protected override void _onDayStart()
        {
            TargetManager.Add(TargetFilter.Living(Match).Except(User));
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday()) TargetManager[0] = null;
            TargetManager[0]?.Role.Ability.DisablePiercing();
        }

        protected override void _onNightStart()
        {
            if (TargetManager.TryDay(0, out var prisoner))
            {
                TargetManager.Add(Executions > 0 ? prisoner : null);

                prisoner.Role.Ability.CurrentlyDeathImmune = true;

                var jail = Match.ChatManager.Open("Jailor", User, prisoner);
                jail.Participants[User].Name = "Jailor";
                Match.ChatManager.DisableExcept(prisoner, jail);
            }
        }

        protected override void _onNightEnd()
        {
            if (TargetManager.Try(0, out var execution) && Executions > 0)
            {
                Visiting = execution;
                ThreatenPiercing(execution);
                Executions--;
            }
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions = 1;
    }
}
