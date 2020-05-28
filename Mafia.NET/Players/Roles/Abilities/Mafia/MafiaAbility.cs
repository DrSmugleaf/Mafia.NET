using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Mafia // TODO: Disguiser
{
    public interface IMafiaAbility
    {
    }

    public abstract class MafiaAbility<T> : BaseAbility<T>, IMafiaAbility where T : class, IMafiaSetup, new()
    {
        public override void Initialize(IPlayer user)
        {
            InitializeBase(user);
            if (TryTransform(out var newAbility))
            {
                newAbility.Initialize(User);
                User = null;
            }
        }

        public override void Chat()
        {
            var chat = Match.Chat.Open<MafiaChat>(MafiaChat.Name);
            var participant = chat.Get(User);
            participant.Muted = false;
            participant.Deaf = false;
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsMafiaTriad;
        }

        public sealed override void OnDayStart()
        {
            if (TryTransform(out var newAbility))
            {
                newAbility.OnDayStart();
                User = null;
            }
            else
            {
                base.OnDayStart();
            }
        }

        protected bool TryTransform(out IAbility newAbility)
        {
            if (Setup is MafiaMinionSetup mafiaSetup &&
                mafiaSetup.BecomesMafiosoIfAlone &&
                AloneTeam())
                User.ChangeRole(Match.Abilities.Ability<Mafioso>());
            else if (Setup is MafiaSuperMinionSetup superMafiaSetup &&
                     superMafiaSetup.ReplacesGodfather &&
                     NoGodfather())
                User.ChangeRole(Match.Setup.Roles.Abilities.Ability<Godfather>());

            newAbility = User.Role.Ability;
            return newAbility != this;
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

    public class MafiaChat : Chat
    {
        public static readonly string Name = "Mafia";

        public MafiaChat() : base(Name)
        {
        }
    }
}