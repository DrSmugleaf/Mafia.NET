using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Registry;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterAbility("Cult Chat", -1)]
public class CultChat : NightChat
{
    public override string ChatId => "Cult"; // TODO
}