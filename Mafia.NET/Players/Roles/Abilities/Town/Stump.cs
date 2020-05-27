namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Stump", typeof(StumpSetup))]
    public class Stump : TownAbility<StumpSetup>
    {
    }

    public class StumpSetup : ITownSetup
    {
    }
}