using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia // TODO: Disguiser
{
    public interface IMafiaAbility : INightChatter
    {
    }

    public abstract class MafiaAbility<T> : BaseAbility<T>, IMafiaAbility where T : IMafiaSetup, new()
    {
        public static readonly string NightChatName = "Mafia";

        public void Chat() => Match.Chat.Open(NightChatName, User);

        protected bool NoGodfather()
        {
            return !Match.LivingPlayers.Values.Any(player => player.Role.Ability is Godfather);
        }

        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsMafiaTriad;

        protected override string GuiltyName() => "Mafia";

        public sealed override void OnDayStart()
        {
            if (Setup is MafiaMinionSetup mafiaSetup && mafiaSetup.BecomesMafiosoIfAlone && AloneTeam())
            {
                User.Role.Ability = Match.Abilities.Ability<Mafioso>(Match, User);
            }
            else if (Setup is MafiaSuperMinionSetup superMafiaSetup && superMafiaSetup.ReplacesGodfather && NoGodfather())
            {
                User.Role.Ability = Match.Abilities.Ability<Godfather>(Match, User);
            }

            base.OnDayStart();
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
