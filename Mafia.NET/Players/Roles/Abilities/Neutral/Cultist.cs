namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class Cultist : GuiltyAbility<CultistSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsCult;
    }

    public class CultistSetup : IAbilitySetup
    {

    }
}
