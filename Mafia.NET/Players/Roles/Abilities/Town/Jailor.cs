using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;

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
        NightTargetRemoveMessage,
        Nickname
    }

    [RegisterAbility("Jailor", typeof(JailorSetup))]
    public class Jailor : TownAbility<JailorSetup>
    {
        public override void Detain()
        {
            if (!TargetManager.TryDay(out var prisoner)) return;
            
            User.Crimes.Add(CrimeKey.Kidnapping);

            var jail = Match.Chat.Open<JailorChat>(JailorChat.Name(prisoner));
            jail.Get(User).Nickname = JailorKey.Nickname;
            jail.Add(prisoner);

            AddTarget(Uses > 0 ? prisoner : null, new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(JailorKey.NightUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(JailorKey.NightUserRemoveMessage),
                TargetAddMessage = target => Notification.Chat(JailorKey.NightTargetAddMessage, target),
                TargetRemoveMessage = target => Notification.Chat(JailorKey.NightTargetRemoveMessage)
            });

            prisoner.Role.Ability.CurrentlyNightImmune = true;
            Match.Chat.DisableExcept(prisoner, jail);

            prisoner.Role.Ability.PiercingBlockedBy(User);
        }

        public override void Kill()
        {
            if (!TargetManager.Try(out var target) ||
                Uses == 0 ||
                !TargetManager.TryDay(out var prisoner) ||
                target != prisoner)
                return;

            Uses--;
            PiercingAttack(target);
        }

        public override void Block()
        {
            if (!TargetManager.Try(out var target)) return;
            target.Role.Ability.PiercingBlockedBy(User);
        }

        protected override void _onDayStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(JailorKey.DayUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(JailorKey.DayUserRemoveMessage),
                UserChangeMessage = (old, current) => Notification.Chat(JailorKey.DayUserChangeMessage, old, current)
            });
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday())
            {
                TargetManager[0] = null;
                User.OnNotification(Notification.Chat(JailorKey.UnableToJail));
            }

            TargetManager[0]?.Role.Ability.PiercingBlockedBy(User);
        }
    }

    public class JailorSetup : ITownSetup, IChargeSetup
    {
        public int Charges { get; set; } = 1;
    }

    public class JailorChat : Chat
    {
        public JailorChat() : base(null)
        {
        }

        public static string Name(IPlayer prisoner)
        {
            return $"Jailor-{prisoner.Number}";
        }
    }
}