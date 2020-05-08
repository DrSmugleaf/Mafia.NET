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
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetMessage() {

            });
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
                AddTarget(Executions > 0 ? prisoner : null, new TargetMessage()
                {
                    UserAddMessage = (target) => $"You will execute {target.Name}.",
                    UserRemoveMessage = (target) => $"You won't execute {target.Name}.",
                    TargetAddMessage = (target) => $"Your captor has decided to execute you.",
                    TargetRemoveMessage = (target) => $"Your captor has decided to let you live."
                });;

                prisoner.Role.Ability.CurrentlyDeathImmune = true;

                var jail = Match.ChatManager.Open("Jailor", User, prisoner);
                jail.Participants[User].Name = "Jailor";
                Match.ChatManager.DisableExcept(prisoner, jail);
            }
        }

        protected override bool _onNightEnd()
        {
            if (TargetManager.Try(0, out var execution) && Executions > 0)
            {
                Executions--;
                ThreatenPiercing(execution);
                return true;
            }

            return false;
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions = 1;
    }
}
