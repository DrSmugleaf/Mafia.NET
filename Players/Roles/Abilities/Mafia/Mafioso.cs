namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Mafioso", typeof(MafiosoSetup))]
    public class Mafioso : MafiaAbility<MafiosoSetup>
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetMessage()
            {
                UserAddMessage = (target) => $"You suggest to kill {target.Name}.",
                UserRemoveMessage = (target) => "You won't suggest anyone.",
                UserChangeMessage = (old, _new) => $"You instead suggest to kill {_new.Name}."
            });
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.Try(0, out var target))
            {
                User.Crimes.Add("Trespassing");
                Threaten(target);

                return true;
            }

            return false;
        }
    }

    public class MafiosoSetup : IMafiaSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = true;
    }
}
