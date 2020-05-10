using System.Collections.Generic;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Matches.Phases
{
    public class DeathsPhase : BasePhase
    {
        public DeathsPhase(IMatch match, uint duration = 10) : base(match, "Deaths", duration, actionable: false)
        {
            VictoryManager = new VictoryManager(Match);
        }

        public VictoryManager VictoryManager { get; }

        public override IPhase NextPhase()
        {
            if (VictoryManager.TryVictory(out var victory)) return new ConclusionPhase(victory, Match);

            return new DiscussionPhase(Match);
        }

        public override void Start()
        {
            Match.Graveyard.SettleThreats();
            Match.Phase.Day++;
            Match.Phase.CurrentTime = Time.DAY;

            if (Match.Graveyard.UndisclosedDeaths.Count == 0)
            {
                End();
                return;
            }

            var notifications = new List<Notification>();
            var startingMessage = Match.Graveyard.UndisclosedDeaths.Count switch
            {
                var x when x < 1 => "",
                var x when x < 2 => "One of us did not survive the night.",
                var x when x < 4 => "Some of us did not survive the night.",
                var x when x < 6 => "Many of us perished last night.",
                var x when x < 8 => "A mass quantity of people died last night.",
                var x when x < 10 => "Most of the entire town was wiped out last night.",
                var x when x < 12 => "A veritable Armageddon decimated the town last night.",
                var x when x < 14 => "Literally the entire town was obliterated last night.",
                var x when x > 14 => "Your setup is shit.",
                _ => ""
            };

            notifications.Add(Notification.Popup(startingMessage));

            foreach (var death in Match.Graveyard.UndisclosedDeaths)
            {
                death.Victim.Alive = false;

                var popupName = $"{death.VictimName} didn't live to see the morning.";
                var popupCause = death.Description;
                var popupRole = death.VictimRole == null
                    ? "We could not determine their role."
                    : $"{death.VictimName}'s role was {death.VictimRole}";

                if (death.LastWill.Length > 0)
                    notifications.AddRange(new[]
                    {
                        Notification.Chat($"{death.VictimName} left us their last will:"),
                        Notification.Chat(death.LastWill)
                    });

                if (death.DeathNote?.Length > 0)
                    notifications.AddRange(new[]
                    {
                        Notification.Chat("We also found a death note near their corpse:"),
                        Notification.Chat(death.DeathNote)
                    });

                notifications.AddRange(new[]
                {
                    Notification.Popup(popupName),
                    Notification.Popup(popupCause),
                    Notification.Popup(popupRole)
                });
            }

            foreach (var notification in notifications)
            foreach (var player in Match.AllPlayers)
                player.OnNotification(notification);

            Match.Graveyard.Disclose();

            base.Start();
        }
    }
}