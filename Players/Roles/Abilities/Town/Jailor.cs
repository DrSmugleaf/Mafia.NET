namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Jailor", typeof(JailSetup))]
    public class Jailor : BaseAbility<JailSetup>
    {
        protected override void _onDayStart()
        {
            TargetManager.Add(TargetFilter.Living(Match).Except(User));
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday()) TargetManager[0] = null;
            TargetManager[0]?.Role.Ability.ActiveNight(false);
        }

        protected override void _onNightStart()
        {
            var prisoner = TargetManager.Day(0);
            if (prisoner == null) return;

            TargetManager.Add(Setup.Executions > 0 ? prisoner : null);

            var jail = Match.ChatManager.Open("Jailor", false, User, prisoner);
            jail.Participants[User].Name = "Jailor";
            Match.ChatManager.DisableExcept(prisoner, jail);
        }

        protected override void _onNightEnd()
        {
            var execution = TargetManager[0];
            if (execution == null) return;
            ThreatenImmunity(execution);
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions = 1;
    }
}
