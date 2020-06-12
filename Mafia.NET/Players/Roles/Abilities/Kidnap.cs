using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Kidnap", -2, typeof(KidnapSetup))]
    public class Kidnap : Detain<KidnapSetup>
    {
        public override void DayStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match).Except(User);
            if (!Setup.CanKidnapTeamMembers) filter = filter.Except(User.Role.Team);
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
                prisoner.Role.Team == User.Role.Team ? null : prisoner,
                TargetNotification.Enum<DetainKey>(this)).Use();

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

    [RegisterSetup]
    public class KidnapSetup : IAbilitySetup
    {
        public bool CanKidnapTeamMembers = false;
    }
}