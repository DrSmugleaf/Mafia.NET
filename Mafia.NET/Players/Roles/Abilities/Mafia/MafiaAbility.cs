using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Mafia // TODO: Disguiser
{
    public interface IMafiaAbility : INightChatter
    {
    }

    public abstract class MafiaAbility<T> : BaseAbility<T>, IMafiaAbility where T : IMafiaSetup, new()
    {
        public static readonly string NightChatId = "Mafia";

        public void Chat()
        {
            Match.Chat.Open(NightChatId, User);
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsMafiaTriad;
        }

        public sealed override void OnDayStart()
        {
            if (Setup is MafiaMinionSetup mafiaSetup && mafiaSetup.BecomesMafiosoIfAlone && AloneTeam())
                User.Role.Ability = Match.Setup.Roles.Abilities.Ability<Mafioso>();
            else if (Setup is MafiaSuperMinionSetup superMafiaSetup && superMafiaSetup.ReplacesGodfather &&
                     NoGodfather()) User.Role.Ability = Match.Setup.Roles.Abilities.Ability<Godfather>();

            base.OnDayStart();
        }

        protected bool NoGodfather()
        {
            return !Match.LivingPlayers.Any(player => player.Role.Ability is Godfather);
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.Mafia;
        }
    }

    public interface IMafiaSetup : IAbilitySetup
    {
    }

    public abstract class MafiaMinionSetup : IMafiaSetup
    {
        public bool BecomesMafiosoIfAlone = true;
    }

    public abstract class MafiaSuperMinionSetup : IMafiaSetup
    {
        public bool ReplacesGodfather = false;
    }
}