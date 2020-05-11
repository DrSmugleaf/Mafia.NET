namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Framer", typeof(FramerSetup))]
    public class Framer : MafiaAbility<FramerSetup>, IMisc
    {
        public void Misc(IPlayer target)
        {
            User.Crimes.Add("Trespassing");
            target.Crimes.Framing = new Framing(Match);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), new TargetNotification
            {
                UserAddMessage = target => $"You will frame {target.Name}.",
                UserRemoveMessage = target => "You won't frame anyone.",
                UserChangeMessage = (old, current) => $"You will instead frame {current.Name}."
            });
        }
    }

    public class FramerSetup : MafiaMinionSetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}