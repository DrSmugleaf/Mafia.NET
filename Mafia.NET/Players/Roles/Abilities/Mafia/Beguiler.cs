using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum BeguilerKey
    {
        SomeoneHide,
        SelfHide,
        HideAt,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Beguiler", typeof(BeguilerSetup))]
    public class Beguiler : MafiaAbility<BeguilerSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var hide = new Hide(this);
            actions.Add(hide);
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            var filter = Setup.CanHideBehindMafia
                ? TargetFilter.Living(Match).Except(User)
                : TargetFilter.Living(Match).Except(User.Role.Team);

            AddTarget(filter, TargetNotification.Enum<BeguilerKey>());
        }
    }

    public class BeguilerSetup : MafiaMinionSetup, IHideSetup
    {
        public bool CanHideBehindMafia = false;
        public bool NotifiesTarget { get; set; } = false;
        public int Uses { get; set; } = 2;
    }
}