using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum ConsigliereKey
    {
        ExactDetect,
        Detect,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Consigliere", typeof(ConsigliereSetup))]
    public class Consigliere : MafiaAbility<ConsigliereSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var investigate = new Investigate(this);
            actions.Add(investigate);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                TargetNotification.Enum<ConsigliereKey>());
        }
    }

    public class ConsigliereSetup : MafiaSuperMinionSetup, IInvestigativeSetup
    {
        public bool DetectsExactRole { get; set; } = false;
    }
}