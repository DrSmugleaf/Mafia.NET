using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Jailor", typeof(JailorSetup))]
    public class Jailor : TownAbility<JailorSetup>, IDetainer, IRoleBlocker, IKiller
    {
        protected override void _onDayStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetMessage()
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

            AddTarget(Uses > 0 ? prisoner : null, new TargetMessage()
            {
                UserAddMessage = (target) => $"You will execute {target.Name}.",
                UserRemoveMessage = (target) => "You changed your mind.",
                TargetAddMessage = (target) => $"{jailor.Name} will execute {target.Name}.",
                TargetRemoveMessage = (target) => $"{jailor.Name} changed their mind."
            });

            prisoner.Role.Ability.CurrentlyDeathImmune = true;
            Match.Chat.DisableExcept(prisoner, jail);

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

    public class JailorSetup : ITownSetup, IChargeSetup
    {
        public int Charges { get; set; } = 1;
    }
}
