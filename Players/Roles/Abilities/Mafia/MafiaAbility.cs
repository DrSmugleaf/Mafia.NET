namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    public class MafiaAbility<T> : BaseAbility<T> where T : IAbilitySetup
    {
        public static readonly string NightChatName = "Mafia";

        public sealed override void OnDayStart()
        {
            if (Setup is MafiaSetup mafiaSetup && AloneTeam() && mafiaSetup.BecomesMafiosoIfAlone)
            {
                User.Role.Ability = AbilityRegistry.Instance.Ability<Mafioso>(Match, User);
            }

            base.OnDayStart();
        }

        public sealed override bool OnDayEnd()
        {
            return base.OnDayEnd();
        }

        public sealed override bool OnNightStart()
        {
            if (Active) Match.ChatManager.Open(NightChatName, User);
            return base.OnNightStart();
        }

        public sealed override bool OnNightEnd()
        {
            return base.OnNightEnd();
        }
    }

    public abstract class MafiaSetup : IAbilitySetup
    {
        public bool BecomesMafiosoIfAlone = true;
    }
}
