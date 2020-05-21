using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum JailorKey
    {
        DayUserAddMessage,
        DayUserRemoveMessage,
        DayUserChangeMessage,
        UnableToJail,
        NightUserAddMessage,
        NightUserRemoveMessage,
        NightTargetAddMessage,
        NightTargetRemoveMessage
    }

    [RegisterAbility("Jailor", typeof(JailorSetup))]
    public class Jailor : TownAbility<JailorSetup>, IDetainer, IRoleBlocker, IKiller
    {
        public void Detain(IPlayer prisoner)
        {
            User.Crimes.Add("Kidnapping");

            var jail = Match.Chat.Open("Jailor", User, prisoner);
            var jailor = jail.Participants[User];
            jailor.Name = "Jailor";

            AddTarget(Uses > 0 ? prisoner : null, new TargetNotification
            {
                UserAddMessage = target => Entry.Chat(JailorKey.NightUserAddMessage, target),
                UserRemoveMessage = target => Entry.Chat(JailorKey.NightUserRemoveMessage),
                TargetAddMessage = target => Entry.Chat(JailorKey.NightTargetAddMessage, target),
                TargetRemoveMessage = target => Entry.Chat(JailorKey.NightTargetRemoveMessage)
            });

            prisoner.Role.Ability.CurrentlyDeathImmune = true;
            Match.Chat.DisableExcept(prisoner, jail);

            prisoner.Role.Ability.PiercingDisable();
        }

        public void Kill(IPlayer target)
        {
            if (Uses == 0 || !TargetManager.TryDay(out var prisoner) || target != prisoner) return;

            Uses--;
            PiercingAttack(target);
        }

        public void Block(IPlayer target)
        {
            target.Role.Ability.PiercingDisable();
        }

        protected override void _onDayStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetNotification
            {
                UserAddMessage = target => Entry.Chat(JailorKey.DayUserAddMessage, target),
                UserRemoveMessage = target => Entry.Chat(JailorKey.DayUserRemoveMessage),
                UserChangeMessage = (old, current) => Entry.Chat(JailorKey.DayUserChangeMessage, old, current)
            });
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday())
            {
                TargetManager[0] = null;
                User.OnNotification(Entry.Chat(JailorKey.UnableToJail));
            }

            TargetManager[0]?.Role.Ability.PiercingDisable();
        }
    }

    public class JailorSetup : ITownSetup, IChargeSetup
    {
        public int Charges { get; set; } = 1;
    }
}