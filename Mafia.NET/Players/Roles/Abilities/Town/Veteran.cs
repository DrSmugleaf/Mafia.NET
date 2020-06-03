using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum VeteranKey
    {
        UserAddMessage,
        UserRemoveMessage
    }

    [RegisterAbility("Veteran", typeof(VeteranSetup))]
    public class Veteran : TownAbility<VeteranSetup>
    {
        public override void Initialize(IPlayer user)
        {
            InitializeBase(user);
            RoleBlockImmune = true;
        }

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var immune = new Vest(this, AttackStrength.Base); // TODO: UsedUpNow Vest Message
            actions.Add(immune);

            var alert = new Alert(this, AttackStrength.Pierce);
            actions.Add(alert);
        }

        protected override void _onNightStart()
        {
            AddTarget(User, TargetNotification.Enum<VeteranKey>());
        }
    }

    public class VeteranSetup : ITownSetup, IRandomExcluded, IUsesSetup
    {
        public bool ExcludedFromRandoms { get; set; } = false;
        public int Uses { get; set; } = 2;
    }
}