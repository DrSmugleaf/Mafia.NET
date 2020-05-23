using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum JanitorKey
    {
        LastWillReveal,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Janitor", typeof(JanitorSetup))]
    public class Janitor : MafiaAbility<JanitorSetup>
    {
        public override void Clean(IPlayer target)
        {
            var targetThreats = Match.Graveyard.Threats.Where(threat => threat.Victim == target).ToList();

            if (targetThreats.Any())
            {
                User.Crimes.Add(CrimeKey.Trespassing);
                User.Crimes.Add(CrimeKey.DestructionOfProperty);
                Uses--;

                var threat = targetThreats.First();
                var lastWill = Notification.Chat(JanitorKey.LastWillReveal, threat.LastWill);

                threat.VictimRole = null;
                threat.LastWill = "";

                User.OnNotification(lastWill);
            }
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match), TargetNotification.Enum<JanitorKey>());
        }
    }

    public class JanitorSetup : MafiaMinionSetup, IChargeSetup
    {
        public int Charges { get; set; } = 2;
    }
}