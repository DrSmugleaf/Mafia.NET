using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum FramerKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Framer", typeof(FramerSetup))]
    public class Framer : MafiaAbility<FramerSetup>, IMisc
    {
        public void Misc(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);
            target.Crimes.Framing = new Framing(Match);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), TargetNotification.Enum<FramerKey>());
        }
    }

    public class FramerSetup : MafiaMinionSetup, IDetectionImmune
    {
        public bool DetectionImmune { get; set; } = false;
    }
}