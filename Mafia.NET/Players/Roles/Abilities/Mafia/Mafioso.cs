namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Mafioso", typeof(MafiosoSetup))]
    public class Mafioso : MafiaAbility<MafiosoSetup>, IKiller
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetNotification()
            {
                UserAddMessage = (target) => $"You suggest to kill {target.Name}.",
                UserRemoveMessage = (target) => "You won't suggest anyone.",
                UserChangeMessage = (old, _new) => $"You instead suggest to kill {_new.Name}."
            });
        }

        public void Kill(IPlayer target)
        {
            User.Crimes.Add("Trespassing");
            Attack(target);
        }
    }

    public class MafiosoSetup : IMafiaSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = true;
    }
}
