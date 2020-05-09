namespace Mafia.NET.Players.Roles.Abilities.Triad
{
    public interface ITriadAbility
    {
    }

    public abstract class TriadAbility<T> : BaseAbility<T>, ITriadAbility where T : ITriadSetup, new()
    {
        public static readonly string NightChatName = "Triad";

        public override bool DetectableBy(ISheriffSetup setup) => setup.DetectsMafiaTriad;

        protected override string GuiltyName() => "Triad";
    }

    public interface ITriadSetup : IAbilitySetup
    {
    }
}
