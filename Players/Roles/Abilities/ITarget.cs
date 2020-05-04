namespace Mafia.NET.Players.Roles.Abilities
{
    public interface ITarget
    {
        bool Targets(IPlayer player);
    }

    public class NoTarget : ITarget
    {
        public NoTarget()
        {
        }

        public bool Targets(IPlayer player) => true;
    }

    public class SingleTarget : ITarget
    {
        public IPlayer Target { get; set; }

        public SingleTarget(IPlayer target)
        {
            Target = target;
        }

        public bool Targets(IPlayer player) => player == Target;
    }

    public class DualTarget : ITarget
    {
        public IPlayer Target1 { get; set; }
        public IPlayer Target2 { get; set; }

        public DualTarget(IPlayer target1, IPlayer target2)
        {
            Target1 = target1;
            Target2 = target2;
        }

        public bool Targets(IPlayer player) => player == Target1 || player == Target2;
    }
}
