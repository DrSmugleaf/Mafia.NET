using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum FramerKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Framer", typeof(FramerSetup))]
    public class Framer : MafiaAbility<FramerSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var frame = new Frame(this);
            actions.Add(frame);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), TargetNotification.Enum<FramerKey>());
        }
    }

    public class FramerSetup : MafiaMinionSetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}