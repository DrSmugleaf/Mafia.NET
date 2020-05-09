using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
#nullable enable
    [RegisterAbility("Godfather", typeof(GodfatherSetup))]
    public class Godfather : MafiaAbility<GodfatherSetup> // TODO: Different message on sending mafioso to kill, relay targeting messages to mafia members
    {
        protected bool TryMafioso(out IPlayer mafioso)
        {
            mafioso = Match.LivingPlayers.Values
                .Where(player => player.Role.Affiliation == User.Role.Affiliation &&
                player.Role.Ability is Mafioso)
                .FirstOrDefault();

            return mafioso != null;
        }

        protected override void _onNightStart()
        {
            if (Setup.CanKillWithoutMafioso || TryMafioso(out _))
            {
                AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetMessage()
                {
                    UserAddMessage = (target) => $"You will kill {target.Name}.",
                    UserRemoveMessage = (target) => $"You won't kill anyone.",
                    UserChangeMessage = (old, _new) => $"You will instead kill {_new.Name}."
                });
            }
        }

        protected override void _beforeNightEnd()
        {
            if (TargetManager.Try(0, out var target) && TryMafioso(out var mafioso))
            {
                mafioso.Role.Ability.TargetManager.ForceSet(target);
                TargetManager.ForceSet(null);
            }
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.Try(0, out var target) && Setup.CanKillWithoutMafioso)
            {
                User.Crimes.Add("Trespassing");
                Threaten(target);

                return true;
            }

            return false;
        }
    }

    public class GodfatherSetup : IMafiaSetup, INightImmune, IRoleBlockImmune, IDetectionImmune
    {
        public bool NightImmune { get; set; } = true;
        public bool RoleBlockImmune { get; set; } = false;
        public bool DetectionImmune { get; set; } = true;
        public bool CanKillWithoutMafioso { get; set; } = true;
    }
}
