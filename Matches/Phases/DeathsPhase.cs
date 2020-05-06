using Mafia.NET.Matches.Chats;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases
{
    public class DeathsPhase : BasePhase
    {
        public DeathsPhase(IMatch match, uint duration = 10) : base(match, "Deaths", duration)
        {
        }

        public override IPhase NextPhase() => new DiscussionPhase(Match);

        public override void Start()
        {
            Match.PhaseManager.Day++;
            Match.PhaseManager.CurrentTime = TimePhase.DAY;

            if (Match.UndisclosedDeaths.Count == 0)
            {
                End();
                return;
            }

            List<Notification> notifications = new List<Notification>();
            string startingMessage;
            switch (Match.UndisclosedDeaths.Count)
            {
                case 0:
                    return;
                case 1:
                    startingMessage = "One of us did not survive the night.";
                    break;
                case 2:
                case 3:
                    startingMessage = "Some of us did not survive the night.";
                    break;
                default:
                    startingMessage = "Last night was a massacre.";
                    break;
            }

            notifications.Add(Notification.Popup(startingMessage));

            foreach (var death in Match.UndisclosedDeaths)
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

            Match.Graveyard.AddRange(Match.UndisclosedDeaths);
            Match.UndisclosedDeaths.Clear();

            base.Start();
        }
    }
}
