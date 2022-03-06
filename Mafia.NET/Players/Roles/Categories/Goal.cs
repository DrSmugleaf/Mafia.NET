using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Categories;

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

    public static Notification VictoryNotification(this Goal goal, IPlayer player)
    {
        return goal switch
        {
            Goal.Town => Notification.Popup(DayKey.TownVictory),
            Goal.Mafia => Notification.Popup(DayKey.MafiaVictory),
            Goal.Triad => Notification.Popup(DayKey.TriadVictory),
            Goal.Benign => Notification.Popup(DayKey.BenignVictory, player.Role),
            Goal.Killing => Notification.Popup(DayKey.KillingVictory, player.Role),
            Goal.Cult => Notification.Popup(DayKey.CultVictory),
            Goal.Evil => Notification.Popup(DayKey.EvilVictory, player.Role),
            _ => Notification.Popup(DayKey.BenignVictory, player.Role)
        };
    }
}