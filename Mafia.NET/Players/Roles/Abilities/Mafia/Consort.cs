using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum ConsortKey
    {
        CantRoleBlock,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Consort", typeof(ConsortSetup))]
    public class Consort : MafiaAbility<ConsortSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var block = new Block(this);
            actions.Add(block);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), TargetNotification.Enum<ConsortKey>());
        }
    }

    public class ConsortSetup : MafiaMinionSetup, IRoleBlockImmune, IBlockSetup
    {
        public bool DetectsBlockImmune { get; set; } = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}