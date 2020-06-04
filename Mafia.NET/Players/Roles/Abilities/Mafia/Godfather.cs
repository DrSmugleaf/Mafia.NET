using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

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
        // TODO: Trespassing crime
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var head = new MafiaHead(this);
            actions.Add(head);
        }

        public bool TryMinion([MaybeNullWhen(false)] out IPlayer minion)
        {
            minion = Match.LivingPlayers
                .FirstOrDefault(player =>
                    player.Role.Team == User.Role.Team &&
                    player.Ability.AbilitySetup is IMafiaSuggester);

            return minion != null;
        }

        protected override void _onNightStart()
        {
            if (Setup.CanKillWithoutMafioso || TryMinion(out _))
                AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                    TargetNotification.Enum<GodfatherKey>());
        }
    }

    public class GodfatherSetup : IMafiaHead, IMafiaSetup, INightImmune, IRoleBlockImmune, IDetectionImmune
    {
        public bool CanKillWithoutMafioso { get; set; } = true;
        public bool DetectionImmune { get; set; } = true;
        public int NightImmunity { get; set; } = (int) AttackStrength.Base;
        public bool RoleBlockImmune { get; set; } = false;
    }
}