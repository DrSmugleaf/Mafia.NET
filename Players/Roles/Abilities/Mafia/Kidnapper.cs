using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Town;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Kidnapper", typeof(KidnapperSetup))]
    public class Kidnapper : MafiaAbility<KidnapperSetup>, IDetainer, IRoleBlocker, IKiller
    {
        public override void Initialize()
        {
            base.Initialize();
            Uses = Match.Abilities.Setup<JailorSetup>().Charges;
        }

        protected override void _onDayStart()
        {
            TargetFilter filter = TargetFilter.Living(Match);
            if (!Setup.CanKidnapMafiaMembers) filter = filter.Except(User.Role.Affiliation);

            AddTarget(filter, new TargetNotification()
            {
                UserAddMessage = (target) => $"You will jail {target.Name}.",
                UserRemoveMessage = (target) => "You won't jail anyone.",
                UserChangeMessage = (old, _new) => $"You will instead jail {_new.Name}."
            });
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday())
            {
                TargetManager[0] = null;
                User.OnNotification(Notification.Chat("You are unable to jail anyone due to today's lynch."));
            }

            TargetManager[0]?.Role.Ability.PiercingDisable();
        }

        public void Detain(IPlayer prisoner)
        {
            User.Crimes.Add("Kidnapping");

            var jail = Match.Chat.Open("Jailor", User, prisoner);
            var jailor = jail.Participants[User];
            jailor.Name = "Jailor";

            var allies = Match.LivingPlayers.Values.Where(player => player.Role.Affiliation == User.Role.Affiliation && player != User);
            jail.Add(allies, true, false);

            AddTarget(prisoner, new TargetNotification()
            {
                UserAddMessage = (target) => $"You will execute {target.Name}.",
                UserRemoveMessage = (target) => "You changed your mind.",
                TargetAddMessage = (target) => $"{jailor.Name} will execute {target.Name}.",
                TargetRemoveMessage = (target) => $"{jailor.Name} changed their mind."
            });

            prisoner.Role.Ability.CurrentlyDeathImmune = true;

            if (prisoner.Role.Affiliation != User.Role.Affiliation)
            {
                Match.Chat.DisableExcept(prisoner, jail);
            }

            prisoner.Role.Ability.PiercingDisable();
        }

        public void Block(IPlayer target)
        {
            target.Role.Ability.PiercingDisable();
        }

        public void Kill(IPlayer target)
        {
            if (Uses == 0) return;

            Uses--;
            PiercingAttack(target);
        }
    }

    public class KidnapperSetup : MafiaMinionSetup
    {
        public bool CanKidnapMafiaMembers = false;
    }
}
