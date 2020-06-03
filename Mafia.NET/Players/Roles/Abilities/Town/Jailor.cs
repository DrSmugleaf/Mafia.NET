using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;

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
        public override void NightStart(in IList<IAbilityAction> actions)
        {
            var jail = new Jail(this);
            actions.Add(jail);
        }

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var release = new Release(this);
            actions.Add(release);

            var execute = new Execute(this, AttackStrength.Pierce)
            {
                Filter = action =>
                    action.TargetManager.Try(out var target) &&
                    action.Uses > 0 &&
                    action.TargetManager.TryDay(out var prisoner) &&
                    target == prisoner
            };

            actions.Add(execute);
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

    public class JailorSetup : ITownSetup, IUsesSetup
    {
        public int Uses { get; set; } = 1;
    }

    public class JailorChat : Chat
    {
        public JailorChat() : base(null)
        {
        }
    }
}