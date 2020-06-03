using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum EscortKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetRoleBlockImmune
    }

    [RegisterAbility("Escort", typeof(EscortSetup))]
    public class Escort : TownAbility<EscortSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var block = new Block(this);
            actions.Add(block);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<EscortKey>());
        }
    }

    public class EscortSetup : ITownSetup, IRoleBlockImmune, IBlockSetup
    {
        public bool DetectsBlockImmune { get; set; } = false;
        public bool RoleBlockImmune { get; set; } = false;
    }
}