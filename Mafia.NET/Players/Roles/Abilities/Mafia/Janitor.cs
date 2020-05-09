using Mafia.NET.Matches.Chats;
using System;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Janitor", typeof(JanitorSetup))]
    public class Janitor : MafiaAbility<JanitorSetup>, ICleaner
    {
        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match), new TargetNotification()
            {
                UserAddMessage = (target) => $"You will sanitize {target.Name}'s death.",
                UserRemoveMessage = (target) => $"You won't sanitize anyone's death.",
                UserChangeMessage = (old, _new) => $"You will instead sanitize {_new.Name}'s death."
            });
        }

        public void Clean(IPlayer target)
        {
            var targetThreats = Match.Graveyard.Threats.Where(threat => threat.Victim == target);

            if (targetThreats.Any())
            {
                User.Crimes.Add("Trespassing");
                User.Crimes.Add("Destruction of property");
                Uses--;

                var threat = targetThreats.First();
                var notification = Notification.Chat($"Your target's last will was:{Environment.NewLine}{threat.LastWill}");

                threat.VictimRole = null;
                threat.LastWill = "";

                User.OnNotification(notification);
            }
        }
    }

    public class JanitorSetup : MafiaMinionSetup, IChargeSetup
    {
        public int Charges { get; set; } = 2;
    }
}
