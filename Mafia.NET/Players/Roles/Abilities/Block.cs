using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum BlockKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetRoleBlockImmune
    }

    [RegisterAbility("Block", 3, typeof(BlockSetup))]
    public class Block : NightEndAbility<BlockSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<BlockKey>(abilities, TargetFilter.Living(Match).Except(User));
            // TODO: consort.yml escort.yml target filters
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Soliciting);

            if (target.Role.Team.Id == "Town")
                User.Crimes.Add(CrimeKey.DisturbingThePeace);

            var blocked = target.Perks.RoleBlock(User);

            if (blocked) return true;

            if (Setup.DetectsBlockImmune)
            {
                var notification = Notification.Chat(Role, BlockKey.TargetRoleBlockImmune);
                User.OnNotification(notification);
            }

            return false;
        }
    }

    public class BlockSetup : IAbilitySetup
    {
        public bool DetectsBlockImmune { get; set; } = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}