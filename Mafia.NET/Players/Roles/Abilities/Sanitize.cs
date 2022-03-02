using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum SanitizeKey
    {
        LastWillReveal,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Sanitize", 8)]
    public class Sanitize : NightEndAbility
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            if (Uses == 0) return;

            SetupTargets<SanitizeKey>(abilities, TargetFilter.Living(Match));
        }

        public override bool Use(IPlayer target)
        {
            if (Uses == 0) return false;

            var targetThreats = Match.Graveyard.Threats
                .Where(threat => threat.Victim == target)
                .ToList();

            if (targetThreats.Any())
            {
                User.Crimes.Add(CrimeKey.Trespassing);
                User.Crimes.Add(CrimeKey.DestructionOfProperty);
                Uses--;

                var threat = targetThreats.First();

                if (threat.LastWill != null)
                {
                    var lastWill = Notification.Chat(Role, SanitizeKey.LastWillReveal, threat.LastWill);
                    User.OnNotification(lastWill);
                }

                threat.VictimRole = null;
                threat.LastWill = null;

                return true;
            }

            return false;
        }
    }
}