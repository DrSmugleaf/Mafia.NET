namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class Cultist : GuiltyAbility<CultistSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsCult;
        }
    }

    public class CultistSetup : IAbilitySetup
    {
    }
}