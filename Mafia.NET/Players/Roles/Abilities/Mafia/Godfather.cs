using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Godfather", typeof(GodfatherSetup))]
    public class
        Godfather : MafiaAbility<GodfatherSetup>, ISwitcher,
            IKiller // TODO: Different message on sending mafioso to kill, relay targeting messages to mafia members
    {
        public void Kill(IPlayer target)
        {
            if (!Setup.CanKillWithoutMafioso) return;
            User.Crimes.Add("Trespassing");
            Attack(target);
        }

        public void Switch()
        {
            if (TargetManager.Try(out var target) && TryMafioso(out var mafioso))
            {
                mafioso.Role.Ability.TargetManager.ForceSet(target);
                TargetManager.ForceSet(null);
            }
        }

        protected bool TryMafioso(out IPlayer mafioso)
        {
            mafioso = Match.LivingPlayers
                .Where(player => player.Role.Team == User.Role.Team &&
                                 player.Role.Ability is Mafioso)
                .FirstOrDefault();

            return mafioso != null;
        }

        protected override void _onNightStart()
        {
            if (Setup.CanKillWithoutMafioso || TryMafioso(out _))
                AddTarget(TargetFilter.Living(Match).Except(User.Role.Team), new TargetNotification
                {
                    UserAddMessage = target => $"You will kill {target.Name}.",
                    UserRemoveMessage = target => "You won't kill anyone.",
                    UserChangeMessage = (old, current) => $"You will instead kill {current.Name}."
                });
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