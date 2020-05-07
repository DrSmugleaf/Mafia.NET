using Mafia.NET.Matches.Chats;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases
{
    public class DeathsPhase : BasePhase
    {
        public VictoryManager VictoryManager { get; }

        public DeathsPhase(IMatch match, uint duration = 10) : base(match, "Deaths", duration)
        {
            VictoryManager = new VictoryManager(Match);
        }

        public override IPhase NextPhase()
        {
            if (VictoryManager.TryVictory(out var victory))
            {
                return new ConclusionPhase(victory, Match);
            }

            return new DiscussionPhase(Match);
        }

        public override void Start()
        {
            Match.PhaseManager.Day++;
            Match.PhaseManager.CurrentTime = TimePhase.DAY;

            if (Match.Graveyard.UndisclosedDeaths.Count == 0)
            {
                End();
                return;
            }

            var notifications = new List<Notification>();
            string startingMessage = Match.Graveyard.UndisclosedDeaths.Count switch
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
                _ => "",
            };

            notifications.Add(Notification.Popup(startingMessage));

            foreach (var death in Match.Graveyard.UndisclosedDeaths)
            {
                death.Victim.Alive = false;

                string popupName = $"{death.Victim.Name} didn't live to see the morning.";
                string popupCause = ""; // TODO
                string popupRole = $"{death.Victim.Name}'s role was {death.Victim.Role.Name}";
                string chatLastWillAuthor = $"{death.Victim.Name} left us his last will:";
                string chatLastWill = death.LastWill;

                notifications.AddRange(new Notification[] {
                    Notification.Popup(popupName),
                    Notification.Popup(popupCause),
                    Notification.Popup(popupRole),
                    Notification.Chat(chatLastWillAuthor),
                    Notification.Chat(chatLastWill)
                });
            }

            foreach (var notification in notifications)
            {
                foreach (var player in Match.AllPlayers.Values)
                {
                    player.OnNotification(notification);
                }
            }

            Match.Graveyard.Disclose();

            base.Start();
        }
    }
}
