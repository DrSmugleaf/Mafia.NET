using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Categories
{
    public enum Goal
    {
        TOWN,
        MAFIA,
        TRIAD,
        BENIGN,
        KILLING,
        CULT,
        EVIL
    }

    public static class GoalExtensions
    {
        public static Goal[] Enemies(this Goal goal)
        {
            return goal switch
            {
                Goal.TOWN => new Goal[] { Goal.MAFIA, Goal.TRIAD, Goal.EVIL, Goal.KILLING, Goal.CULT },
                Goal.MAFIA => new Goal[] { Goal.TOWN, Goal.TRIAD, Goal.KILLING, Goal.CULT },
                Goal.TRIAD => new Goal[] { Goal.TOWN, Goal.MAFIA, Goal.KILLING, Goal.CULT },
                Goal.KILLING => new Goal[] { Goal.TOWN, Goal.MAFIA, Goal.TRIAD, Goal.KILLING, Goal.CULT },
                Goal.CULT => new Goal[] { Goal.TOWN, Goal.MAFIA, Goal.TRIAD, Goal.KILLING },
                Goal.EVIL => new Goal[] { Goal.TOWN },
                _ => new Goal[] { }
            };
        }

        public static Notification VictoryNotification(this Goal goal, IPlayer player)
        {
            return Notification.Popup(goal switch
            {
                Goal.TOWN => "The Town has won!",
                Goal.MAFIA => "The Mafia have won!",
                Goal.TRIAD => "The Triad have won!",
                Goal.BENIGN => $"The {player.Role.Name} has won!",
                Goal.KILLING => $"The {player.Role.Name} has won!",
                Goal.CULT => $"The Cult has won!",
                Goal.EVIL => $"The {player.Role.Name} has won!",
                _ => ""
            });
        }
    }
}
