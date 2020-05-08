using System;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    public class MafiaAbility<T> : BaseAbility<T> where T : IAbilitySetup
    {
        public static readonly string NightChatName = "Mafia";

        protected bool NoGodfather ()
        {
            return !Match.LivingPlayers.Values.Any(player => player.Role.Ability is Godfather);
        }

        public sealed override void OnDayStart()
        {
            if (Setup is MafiaMinionSetup mafiaSetup && mafiaSetup.BecomesMafiosoIfAlone && AloneTeam())
            {
                User.Role.Ability = Match.Abilities.Ability<Mafioso>(Match, User);
            }

            if (Setup is MafiaSuperMinionSetup superMafiaSetup && superMafiaSetup.ReplacesGodfather && NoGodfather())
            {
                User.Role.Ability = Match.Abilities.Ability<Godfather>(Match, User);
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

        public sealed override bool AfterNightEnd()
        {
            return base.AfterNightEnd();
        }
    }

    public abstract class MafiaMinionSetup : IAbilitySetup
    {
        public bool BecomesMafiosoIfAlone = true;
    }

    public abstract class MafiaSuperMinionSetup : IAbilitySetup
    {
        public bool ReplacesGodfather = false;
    }
}
