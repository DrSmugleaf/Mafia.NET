using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum KidnapperKey
    {
        DayUserAddMessage,
        DayUserRemoveMessage,
        DayUserChangeMessage,
        UnableToJail,
        NightUserAddMessage,
        NightUserRemoveMessage,
        NightTargetAddMessage,
        NightTargetRemoveMessage,
        Nickname
    }

    [RegisterAbility("Kidnapper", typeof(KidnapperSetup))]
    public class Kidnapper : MafiaAbility<KidnapperSetup>
    {
        public override void Initialize(IPlayer user)
        {
            base.Initialize(user);
            Uses = Match.AbilitySetups.Setup<JailorSetup>().Charges;
        }

        public void Detain(IPlayer prisoner)
        {
            User.Crimes.Add(CrimeKey.Kidnapping);

            var jail = Match.Chat.Open<JailorChat>(JailorChat.Name(prisoner));
            jail.Get(User).Nickname = KidnapperKey.Nickname;
            jail.Add(prisoner);

            var allies = Match.LivingPlayers.Where(player =>
                player.Role.Team == User.Role.Team && player != User);
            jail.Add(allies, true);

            AddTarget(prisoner.Role.Team == User.Role.Team ? null : prisoner, new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(KidnapperKey.NightUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(KidnapperKey.NightUserRemoveMessage),
                TargetAddMessage = target => Notification.Chat(KidnapperKey.NightTargetAddMessage, target),
                TargetRemoveMessage = target => Notification.Chat(KidnapperKey.NightTargetRemoveMessage)
            });

            prisoner.Role.Ability.CurrentlyDeathImmune = true;

            if (prisoner.Role.Team != User.Role.Team) Match.Chat.DisableExcept(prisoner, jail);

            prisoner.Role.Ability.PiercingBlockedBy(User);
        }

        public override void Kill(IPlayer target)
        {
            if (Uses == 0 || !TargetManager.TryDay(out var prisoner) || target != prisoner) return;

            Uses--;
            PiercingAttack(target);
        }

        public override void Block(IPlayer target)
        {
            target.Role.Ability.PiercingBlockedBy(User);
        }

        protected override void _onDayStart()
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanKidnapMafiaMembers) filter = filter.Except(User.Role.Team);

            AddTarget(filter, new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(KidnapperKey.DayUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(KidnapperKey.DayUserRemoveMessage),
                UserChangeMessage = (old, current) => Notification.Chat(KidnapperKey.DayUserChangeMessage, old, current)
            });
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday())
            {
                TargetManager[0] = null;
                User.OnNotification(Notification.Chat(KidnapperKey.UnableToJail));
            }

            TargetManager[0]?.Role.Ability.PiercingBlockedBy(User);
        }
    }

    public class KidnapperSetup : MafiaMinionSetup
    {
        public bool CanKidnapMafiaMembers = false;
    }
}