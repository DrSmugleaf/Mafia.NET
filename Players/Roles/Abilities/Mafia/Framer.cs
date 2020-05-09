namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Framer", typeof(FramerSetup))]
    public class Framer : MafiaAbility<FramerSetup>, IMisc
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetNotification()
            {
                UserAddMessage = (target) => $"You will frame {target.Name}.",
                UserRemoveMessage = (target) => "You won't frame anyone.",
                UserChangeMessage = (old, _new) => $"You will instead frame {_new.Name}."
            });
        }

        public void Misc(IPlayer target)
        {
            User.Crimes.Add("Trespassing");
            target.Crimes.Framing = new Framing(Match);
        }
    }

    public class FramerSetup : MafiaMinionSetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}
