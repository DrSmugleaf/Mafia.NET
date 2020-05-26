using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum MafiosoKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Mafioso", typeof(MafiosoSetup))]
    public class Mafioso : MafiaAbility<MafiosoSetup>
    {
        public override void Kill(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);
            AttackedBy(target);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), TargetNotification.Enum<MafiosoKey>());
            // TODO: Change messages when alone
        }
    }

    public class MafiosoSetup : IMafiaSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = true;
    }
}