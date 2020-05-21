using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Categories
{
    public enum Goal
    {
        Town,
        Mafia,
        Triad,
        Benign,
        Killing,
        Cult,
        Evil
    }

    public static class GoalExtensions
    {
        public static Goal[] Enemies(this Goal goal)
        {
            return goal switch
            {
                Goal.Town => new[] {Goal.Mafia, Goal.Triad, Goal.Evil, Goal.Killing, Goal.Cult},
                Goal.Mafia => new[] {Goal.Town, Goal.Triad, Goal.Killing, Goal.Cult},
                Goal.Triad => new[] {Goal.Town, Goal.Mafia, Goal.Killing, Goal.Cult},
                Goal.Killing => new[] {Goal.Town, Goal.Mafia, Goal.Triad, Goal.Killing, Goal.Cult},
                Goal.Cult => new[] {Goal.Town, Goal.Mafia, Goal.Triad, Goal.Killing},
                Goal.Evil => new[] {Goal.Town},
                _ => new Goal[] { }
            };
        }

        public static Entry VictoryNotification(this Goal goal, IPlayer player)
        {
            return goal switch
            {
                Goal.Town => Entry.Popup(DayKey.TownVictory),
                Goal.Mafia => Entry.Popup(DayKey.MafiaVictory),
                Goal.Triad => Entry.Popup(DayKey.TriadVictory),
                Goal.Benign => Entry.Popup(DayKey.BenignVictory, player.Role),
                Goal.Killing => Entry.Popup(DayKey.KillingVictory, player.Role),
                Goal.Cult => Entry.Popup(DayKey.CultVictory),
                Goal.Evil => Entry.Popup(DayKey.EvilVictory, player.Role),
                _ => Entry.Popup(DayKey.BenignVictory, player.Role)
            };
        }
    }
}