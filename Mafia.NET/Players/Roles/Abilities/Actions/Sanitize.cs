using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum SanitizeKey
    {
        LastWillReveal
    }

    public class Sanitize : AbilityAction<IUsesSetup>
    {
        public Sanitize(
            IAbility<IUsesSetup> ability,
            int priority = 8,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            var targetThreats = Match.Graveyard.Threats
                .Where(threat => threat.Victim == target)
                .ToList();

            if (targetThreats.Any())
            {
                User.Crimes.Add(CrimeKey.Trespassing);
                User.Crimes.Add(CrimeKey.DestructionOfProperty);
                Ability.Uses--;

                var threat = targetThreats.First();
                var lastWill = Notification.Chat(Ability, SanitizeKey.LastWillReveal, threat.LastWill);

                threat.VictimRole = null;
                threat.LastWill = "";

                User.OnNotification(lastWill);

                return true;
            }

            return false;
        }
    }
}