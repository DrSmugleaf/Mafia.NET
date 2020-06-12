using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum ControlKey
    {
        NoTargets,
        ControlAdd,
        SelfAdd,
        BothAdd,
        TargetStopped, // TODO
        TargetControlled,
        ForcedSuicide, // TODO
        UserTargetControlled,
        UserTargetSelfTarget
    }

    [RegisterAbility("Control", 2, typeof(ControlSetup))]
    public class Control : NightEndAbility<IControlSetup>
    {
        public Notification UserMessage()
        {
            var first = Targets[0];
            var second = Targets[1];

            if (first == null && second == null) return Notification.Chat(Role, ControlKey.NoTargets);
            if (first != null && second == null) return Notification.Chat(Role, ControlKey.ControlAdd, first);
            if (first == second) return Notification.Chat(Role, ControlKey.SelfAdd, first);
            if (first != null) return Notification.Chat(Role, ControlKey.BothAdd, first, second);
            return Notification.Empty;
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match).Except(User);
            if (!Setup.CanCauseSelfTargets) filter = filter.Except(Targets);

            var notification = new TargetNotification
            {
                UserAddMessage = target => UserMessage(),
                UserRemoveMessage = target => UserMessage(),
                UserChangeMessage = (old, current) => UserMessage()
            };

            SetupTargets(abilities, filter, notification);
            SetupTargets(abilities, filter, notification);
        }

        public override bool Use(IPlayer first, IPlayer second)
        {
            first.Targets.ForceSet(second);
            
            var userNotification = first == second
                ? Notification.Chat(Role, ControlKey.UserTargetSelfTarget, first)
                : Notification.Chat(Role, ControlKey.UserTargetControlled, first, second);
            User.OnNotification(userNotification);

            if (Setup.VictimKnows)
            {
                var targetNotification = Notification.Chat(Role, ControlKey.TargetControlled);
                first.OnNotification(targetNotification);
            }

            return true;
        }
    }

    public interface IControlSetup : IAbilitySetup
    {
        bool CanCauseSelfTargets { get; set; }
        bool VictimKnows { get; set; }
        bool WitchDoctorWhenConverted { get; set; } // TODO
    }

    [RegisterSetup]
    public class ControlSetup : IControlSetup
    {
        public bool CanCauseSelfTargets { get; set; } = true;
        public bool VictimKnows { get; set; } = true;
        public bool WitchDoctorWhenConverted { get; set; } = true;
    }
}