using System.Linq;
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
        public override void Kill()
        {
            if (!TargetManager.Try(out var target)) return;

            var otherMafiosoAttacked = Match.Graveyard.ThreatsOn(target)
                .Any(threat => threat.Killer?.Role.Ability is Mafioso);
            if (otherMafiosoAttacked) return;

            User.Crimes.Add(CrimeKey.Trespassing);
            Attack(target);
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