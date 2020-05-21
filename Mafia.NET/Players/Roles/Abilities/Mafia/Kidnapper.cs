﻿using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
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
        NightTargetRemoveMessage
    }

    [RegisterAbility("Kidnapper", typeof(KidnapperSetup))]
    public class Kidnapper : MafiaAbility<KidnapperSetup>, IDetainer, IRoleBlocker, IKiller
    {
        public override void Initialize(IMatch match, IPlayer user)
        {
            base.Initialize(match, user);
            Uses = Match.Setup.Roles.Abilities.Setup<JailorSetup>().Charges;
        }

        public void Detain(IPlayer prisoner)
        {
            User.Crimes.Add("Kidnapping");

            var jail = Match.Chat.Open("Jailor", User, prisoner); // TODO: Localize "Jailor"
            var detainer = jail.Participants[User];
            detainer.Name = "Jailor";

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

            TargetManager[0]?.Role.Ability.PiercingDisable();
        }
    }

    public class KidnapperSetup : MafiaMinionSetup
    {
        public bool CanKidnapMafiaMembers = false;
    }
}