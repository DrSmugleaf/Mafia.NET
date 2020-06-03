using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum VigilanteKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetImmune,
        FirstNight
    }

    [RegisterAbility("Vigilante", typeof(VigilanteSetup))]
    public class Vigilante : TownAbility<VigilanteSetup>
    {
        public TargetNotification TargetMessage()
        {
            return Match.Phase.Day == 1
                ? new TargetNotification
                {
                    UserAddMessage = target =>
                    {
                        TargetManager.ForceSet(null);
                        return Notification.Chat(VigilanteKey.FirstNight);
                    }
                }
                : TargetNotification.Enum<VigilanteKey>();
        }

        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var shoot = new Shoot(this, AttackStrength.Base);
            actions.Add(shoot);
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match).Except(User), TargetMessage());
        }
    }

    public class VigilanteSetup : ITownSetup, IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}