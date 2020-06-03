using System.Linq;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Mafia;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Kidnap : Detain
    {
        public Kidnap(
            IAbility ability,
            int priority = -2,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer prisoner)
        {
            if (!base.Use()) return false;

            Ability.AddTarget(prisoner.Role.Team == User.Role.Team ? null : prisoner, new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(KidnapperKey.NightUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(KidnapperKey.NightUserRemoveMessage),
                TargetAddMessage = target => Notification.Chat(KidnapperKey.NightTargetAddMessage, target),
                TargetRemoveMessage = target => Notification.Chat(KidnapperKey.NightTargetRemoveMessage)
            });

            var chatId = $"Jailor-{prisoner.Number}";
            var jail = Match.Chat.Open(chatId);
            Match.Chat.DisableForExcept(prisoner, jail);

            var allies = Match.LivingPlayers.Where(player =>
                player.Role.Team == User.Role.Team &&
                player != User);
            jail.Add(allies, true);

            if (prisoner.Role.Team != User.Role.Team)
                Match.Chat.DisableForExcept(prisoner, jail);

            return true;
        }
    }
}