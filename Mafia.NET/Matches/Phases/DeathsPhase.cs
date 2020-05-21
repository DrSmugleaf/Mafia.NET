using System;
using Mafia.NET.Localization;

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
            Match.Phase.CurrentTime = Time.Day;

            if (Match.Graveyard.UndisclosedDeaths.Count == 0)
            {
                End();
                return;
            }

            var startingMessage = Entry.Popup(Match.Graveyard.UndisclosedDeaths.Count switch
            {
                var x when x < 2 => DayKey.Deaths1,
                var x when x < 4 => DayKey.Deaths3,
                var x when x < 6 => DayKey.Deaths5,
                var x when x < 8 => DayKey.Deaths7,
                var x when x < 10 => DayKey.Deaths9,
                var x when x < 12 => DayKey.Deaths11,
                var x when x < 14 => DayKey.Deaths13,
                var x when x >= 14 => DayKey.Deaths15,
                _ => throw new NotImplementedException()
            });

            var deaths = new EntryBundle();
            foreach (var death in Match.Graveyard.UndisclosedDeaths)
            {
                death.Victim.Alive = false;

                deaths.Popup(DayKey.DeathMorning, death.Victim); // TODO: Randomize
                // deaths.Add(death.Description); // TODO
                deaths.Popup(death.VictimRole == null ? DayKey.DeathRoleUnknown : DayKey.DeathRoleReveal, death.Victim,
                    death.Victim.Role); // TODO

                if (death.LastWill.Length > 0)
                {
                    deaths.Chat(DayKey.LastWillAuthor, death.Victim);
                    deaths.Chat(DayKey.LastWillContent, death.LastWill);
                }

                if (death.DeathNote?.Length > 0)
                {
                    deaths.Chat(DayKey.DeathNote);
                    deaths.Chat(DayKey.DeathNoteContent, death.DeathNote);
                }
            }

            foreach (var player in Match.AllPlayers)
            {
                player.OnNotification(startingMessage);
                player.OnNotification(deaths);
            }

            Match.Graveyard.Disclose();

            base.Start();
        }
    }
}