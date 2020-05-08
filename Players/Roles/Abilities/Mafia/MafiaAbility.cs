namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    public class MafiaAbility<T> : BaseAbility<T> where T : IAbilitySetup
    {
        public static readonly string NightChatName = "Mafia";

        public sealed override void OnDayStart()
        {
            base.OnDayStart();
        }

        public sealed override bool OnDayEnd()
        {
            base.OnDayEnd();
            return Active;
        }

        public sealed override bool OnNightStart()
        {
            if(base.OnNightStart()) Match.ChatManager.Open(NightChatName, User);
            return Active;
        }

        public sealed override bool OnNightEnd()
        {
            base.OnNightEnd();
            return Active;
        }
    }
}
