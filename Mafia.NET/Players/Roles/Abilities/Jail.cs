using System.Collections.Generic;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Jail", -2, typeof(DetainSetup))]
    public class Jail : Detain
    {
        public override void DayStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match).Except(User);
            var notification = new TargetNotification
            {
                UserAddMessage = target => Notification.Chat(Role, DetainKey.DayUserAddMessage, target),
                UserRemoveMessage = target => Notification.Chat(Role, DetainKey.DayUserRemoveMessage),
                UserChangeMessage = (old, current) =>
                    Notification.Chat(Role, DetainKey.DayUserChangeMessage, old, current)
            };

            SetupTargets(abilities, filter, notification);
        }

        public override bool Use(IPlayer prisoner)
        {
            if (!base.Use(prisoner)) return false;

            SetupTargets(
                Uses > 0 ? prisoner : null,
                TargetNotification.Enum<DetainKey>(this)).Use();

            var chatId = $"Jailor-{prisoner.Number}";
            var jail = Match.Chat.Open(chatId);
            Match.Chat.DisableForExcept(prisoner, jail);

            return true;
        }
    }
}