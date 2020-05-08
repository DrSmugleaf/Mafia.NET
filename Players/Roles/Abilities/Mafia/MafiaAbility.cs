﻿namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    public class MafiaAbility<T> : BaseAbility<T> where T : IAbilitySetup
    {
        public static readonly string NightChatName = "Mafia";

        public sealed override void OnDayStart()
        {
            if (Setup is MafiaMinionSetup mafiaSetup && AloneTeam() && mafiaSetup.BecomesMafiosoIfAlone)
            {
                User.Role.Ability = Match.Abilities.Ability<Mafioso>(Match, User);
            }

            base.OnDayStart();
        }

        public sealed override bool OnDayEnd()
        {
            return base.OnDayEnd();
        }

        public sealed override bool OnNightStart()
        {
            if (Active) Match.Chat.Open(NightChatName, User);
            return base.OnNightStart();
        }

        public sealed override bool OnNightEnd()
        {
            return base.OnNightEnd();
        }
    }

    public abstract class MafiaMinionSetup : IAbilitySetup
    {
        public bool BecomesMafiosoIfAlone = true;
    }
}