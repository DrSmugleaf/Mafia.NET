using System;
using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum AuditKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        UserAudited,
        TargetAudited,
        CantAudit,
        AuditsLeft,
        AuditsLeftPlural,
        OutOfAudits,
        AlreadyAudited
    }

    [RegisterAbility("Audit", 11, typeof(AuditSetup))]
    public class Audit : NightEndAbility<IAuditSetup>
    {
        public bool Auditable(IPlayer target)
        {
            var role = target.Role.Id;

            return !AlreadyAudited(target) &&
                   target.Alive &&
                   target.Role.Perks.CurrentDefense <= AttackStrength.None &&
                   role != "Cultist" && // TODO
                   role != "Witch Doctor";
        }

        public bool AlreadyAudited(IPlayer target)
        {
            var role = target.Role.Id;

            return role == "Citizen" || // TODO
                   role == "Mafioso" ||
                   role == "Enforcer" ||
                   role == "Scumbag";
        }

        public IRole AuditedRole(IPlayer target)
        {
            var team = target.Role.Team.Id;

            var role = team switch
            {
                "Town" => Match.Roles["Citizen"], // TODO
                "Mafia" => Match.Roles["Mafioso"],
                "Triad" => Match.Roles["Enforcer"],
                "Neutral" => Match.Roles["Scumbag"],
                _ => throw new NotImplementedException()
            };

            return role.Build();
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<AuditKey>(abilities, TargetFilter.Living(Match).Except(User));

            if (Uses == 0)
            {
                User.OnNotification(Notification.Chat(Role, AuditKey.OutOfAudits));
                return;
            }

            var usesLeft = Notification.Chat(Role, Uses == 1 ? AuditKey.AuditsLeft : AuditKey.AuditsLeftPlural, Uses);
            User.OnNotification(usesLeft);
        }

        public override bool CanUseAny()
        {
            return base.CanUseAny() && Uses > 0;
        }

        public override bool Use(IPlayer target)
        {
            if (Uses == 0) return false;

            if (AlreadyAudited(target))
            {
                var notification = Notification.Chat(Role, AuditKey.AlreadyAudited, target);
                User.OnNotification(notification);

                return false;
            }

            if (Auditable(target))
            {
                Uses--;

                var role = AuditedRole(target);
                target.ChangeRole(role);

                var userAudited = Notification.Chat(Role, AuditKey.UserAudited, target);
                User.OnNotification(userAudited);

                var targetAudited = Notification.Chat(Role, AuditKey.TargetAudited);
                User.OnNotification(targetAudited);

                return true;
            }

            {
                var notification = Notification.Chat(Role, AuditKey.CantAudit, target);
                User.OnNotification(notification);

                return false;
            }
        }
    }

    public interface IAuditSetup : IUsesSetup
    {
        bool ConvertsMafiaToMafioso { get; }
        bool ConvertsTriadToEnforcer { get; }
        bool ImmunityPreventsConversion { get; }
    }

    public class AuditSetup : IAuditSetup
    {
        public int Uses { get; set; } = 3;
        public bool ConvertsMafiaToMafioso { get; } = true;
        public bool ConvertsTriadToEnforcer { get; } = true;
        public bool ImmunityPreventsConversion { get; } = true;
    }
}