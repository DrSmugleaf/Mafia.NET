using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Jail : Detain
    {
        public Jail(
            IAbility ability,
            int priority = -2,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool ResolveUse()
        {
            TargetManager.TryDay(out var first);
            TargetManager.TryDay(1, out var second);

            if (first == null && second == null) return Use();
            if (first != null && second != null) return Use(first, second);
            if (first != null) return Use(first);
            return false;
        }

        public override bool Use(IPlayer prisoner)
        {
            if (!base.Use(prisoner)) return false;

            Ability.AddTarget(Uses > 0 ? prisoner : null, new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(Ability, DetainKey.NightUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(Ability, DetainKey.NightUserRemoveMessage),
                TargetAddMessage = target => Notification.Chat(Ability, DetainKey.NightTargetAddMessage, target),
                TargetRemoveMessage = target => Notification.Chat(Ability, DetainKey.NightTargetRemoveMessage)
            });

            var chatId = $"Jailor-{prisoner.Number}";
            var jail = Match.Chat.Open(chatId);
            Match.Chat.DisableForExcept(prisoner, jail);

            return true;
        }
    }
}