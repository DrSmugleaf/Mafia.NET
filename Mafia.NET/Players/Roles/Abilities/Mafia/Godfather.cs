using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum GodfatherKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Godfather", typeof(GodfatherSetup))]
    public class Godfather : MafiaAbility<GodfatherSetup>
    {
        // TODO: Different message on sending mafioso to kill, relay targeting messages to mafia members
        public override void Kill()
        {
            if (!TargetManager.Try(out var target) ||
                !Setup.CanKillWithoutMafioso) return;
            User.Crimes.Add(CrimeKey.Trespassing);
            Attack(target);
        }

        public override void Switch()
        {
            if (TargetManager.Try(out var target) && TryMafioso(out var mafioso))
            {
                mafioso.Role.Ability.TargetManager.ForceSet(target);
                TargetManager.ForceSet(null);
            }
        }

        protected bool TryMafioso([CanBeNull] [NotNullWhen(true)] out IPlayer mafioso)
        {
            mafioso = Match.LivingPlayers
                .FirstOrDefault(player => player.Role.Team == User.Role.Team && player.Role.Ability is Mafioso);

            return mafioso != null;
        }

        protected override void _onNightStart()
        {
            if (Setup.CanKillWithoutMafioso || TryMafioso(out _))
                AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                    TargetNotification.Enum<GodfatherKey>());
        }
    }

    public class GodfatherSetup : IMafiaSetup, INightImmune, IRoleBlockImmune, IDetectionImmune
    {
        public bool CanKillWithoutMafioso { get; set; } = true;
        public bool DetectionImmune { get; set; } = true;
        public bool NightImmune { get; set; } = true;
        public bool RoleBlockImmune { get; set; } = false;
    }
}