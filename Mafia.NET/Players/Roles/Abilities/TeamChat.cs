using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Team Chat", -1)]
    public class TeamChat : NightChat
    {
        public override string ChatId => User.Role.Team.Id; // TODO
    }
}