using Mafia.NET.Players.Roles.Abilities.Bases;

namespace Mafia.NET.Players.Roles.Abilities.Actions;

public class Transform : DayStartAbility
{
    public Transform(IRole newRole)
    {
        Priority = -3;
        NewRole = newRole;
    }

    public Transform() : this(null!)
    {
    }

    public IRole NewRole { get; set; }

    public override bool Use()
    {
        User.ChangeRole(NewRole); // TODO: Update current ability iterator after transform?
        return true;
    }
}