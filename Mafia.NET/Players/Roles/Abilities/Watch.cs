using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum WatchKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        SomeoneVisitedTarget,
        NoneVisitedTarget
    }

    [RegisterAbility("Watch", 9, typeof(WatchSetup))]
    public class Watch : NightEndAbility<IWatchSetup>
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf) filter = filter.Except(User);

            SetupTargets<WatchKey>(abilities, filter);
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            var foreignVisits = new EntryBundle();
            foreach (var other in Match.LivingPlayers)
                if (other != User &&
                    other.Role.DetectionProfile.TryDetectTarget(out var foreignTarget, Setup) &&
                    foreignTarget == target)
                    foreignVisits.Chat(Role, WatchKey.SomeoneVisitedTarget, other);

            if (foreignVisits.Entries.Count == 0)
                foreignVisits.Chat(Role, WatchKey.NoneVisitedTarget);

            User.OnNotification(foreignVisits);

            return true;
        }
    }

    public interface IWatchSetup : IDetectSetup
    {
        bool CanTargetSelf { get; set; }
    }

    public class WatchSetup : IWatchSetup
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
        public bool CanTargetSelf { get; set; } = false;
    }
}