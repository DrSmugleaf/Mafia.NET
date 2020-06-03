using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
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
            InitializeBase(user);

            if (TryTransform(out var newAbility))
            {
                newAbility.Initialize(user);
                User = null;
            }
            else
            {
                Uses = Match.AbilitySetups.Setup<JailorSetup>().Uses;
            }
        }

        public override void NightStart(in IList<IAbilityAction> actions)
        {
            var kidnap = new Kidnap(this);
            actions.Add(kidnap);

            base.NightStart(in actions);
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