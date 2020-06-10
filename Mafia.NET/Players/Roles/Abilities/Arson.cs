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
    public enum ArsonKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        DouserDouse,
        OtherDouse,
        UnDouse,
        Ignite,
        DouseTarget
    }

    [RegisterAbility("Arson", 5, typeof(ArsonSetup))]
    public class Arson : NightEndAbility<ArsonSetup>
    {
        public Notification UserAddMessage(IPlayer target)
        {
            return target == User
                ? Notification.Chat(Role, ArsonKey.Ignite)
                : Notification.Chat(Role, ArsonKey.UserAddMessage, target);
        }

        public bool Douse(IPlayer target)
        {
            target.Perks.Doused = true;

            if (Setup.VictimNoticesDousing || target.Abilities.Any<Arson>())
            {
                var notification = target.Abilities.Any<Arson>()
                    ? Notification.Chat(Role, ArsonKey.DouserDouse)
                    : Notification.Chat(Role, ArsonKey.OtherDouse);

                target.OnNotification(notification);
            }

            return true;
        }

        public bool Ignite()
        {
            foreach (var player in Match.LivingPlayers)
            {
                if (!player.Perks.Doused) continue;

                var stoppable = !Setup.IgnitionAlwaysKills;
                var strength = Setup.IgnitionAlwaysKills
                    ? AttackStrength.Pierce
                    : AttackStrength.Base;

                var attack = Attack(strength, Priority, false, stoppable);
                attack.Use(player);

                if (Setup.IgnitionKillsVictimsTargets &&
                    player.Targets.Try(out var victimsTarget))
                    attack.Use(victimsTarget);
            }

            return true;
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match);
            var notification = TargetNotification.Enum<ArsonKey>(this);
            notification.UserAddMessage = UserAddMessage;

            SetupTargets(abilities, filter, notification);
        }

        public override bool CanUse(IPlayer target)
        {
            return base.CanUse(target) || Setup.DousesRoleBlockers;
        }

        public override bool Use()
        {
            User.Perks.Doused = false;
            var notification = Notification.Chat(Role, ArsonKey.UnDouse);
            User.OnNotification(notification);

            return true;
        }

        public override bool Use(IPlayer target)
        {
            if (target != User && !RoleBlocked) Douse(target);

            if (Setup.DousesRoleBlockers)
                foreach (var blocker in User.Perks.RoleBlockers)
                    Douse(blocker);

            if (target == User && !RoleBlocked) return Ignite();

            return true;
        }
    }

    public class ArsonSetup : IAbilitySetup
    {
        public bool DousesRoleBlockers { get; set; } = true;
        public bool IgnitionAlwaysKills { get; set; } = true;
        public bool IgnitionKillsVictimsTargets { get; set; } = true;
        public bool VictimNoticesDousing { get; set; } = true;
    }
}