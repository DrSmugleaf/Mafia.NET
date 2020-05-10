using System;
using System.Linq;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Janitor", typeof(JanitorSetup))]
    public class Janitor : MafiaAbility<JanitorSetup>, ICleaner
    {
        public void Clean(IPlayer target)
        {
            var targetThreats = Match.Graveyard.Threats.Where(threat => threat.Victim == target);

            if (targetThreats.Any())
            {
                User.Crimes.Add("Trespassing");
                User.Crimes.Add("Destruction of property");
                Uses--;

                var threat = targetThreats.First();
                var notification =
                    Notification.Chat($"Your target's last will was:{Environment.NewLine}{threat.LastWill}");

                threat.VictimRole = null;
                threat.LastWill = "";

                User.OnNotification(notification);
            }
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match), new TargetNotification
            {
                UserAddMessage = target => $"You will sanitize {target.Name}'s death.",
                UserRemoveMessage = target => "You won't sanitize anyone's death.",
                UserChangeMessage = (old, current) => $"You will instead sanitize {current.Name}'s death."
            });
        }
    }

    public class JanitorSetup : MafiaMinionSetup, IChargeSetup
    {
        public int Charges { get; set; } = 2;
    }
}