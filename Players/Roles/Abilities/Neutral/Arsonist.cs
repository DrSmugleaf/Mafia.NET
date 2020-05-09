namespace Mafia.NET.Players.Roles.Abilities.Neutral
{
    public class Arsonist : GuiltyAbility<ArsonistSetup>
    {
        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsArsonist;
    }

    public class ArsonistSetup : IAbilitySetup
    {

    }
}
