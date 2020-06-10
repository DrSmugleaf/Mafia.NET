using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum MafiaHeadKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Mafia Head", 4, typeof(MafiaHeadSetup))]
    public class MafiaHead : NightEndAbility<MafiaHeadSetup>
    {
        // TODO: Different message on sending mafioso to kill, relay targeting messages to mafia members
        // TODO: Trespassing crime
        public bool TrySuggester([MaybeNullWhen(false)] out IPlayer suggester)
        {
            suggester = Match.LivingPlayers
                .FirstOrDefault(player =>
                    player.Role.Team == User.Role.Team &&
                    player.Abilities.Any<MafiaSuggest>());

            return suggester != null;
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            if (!Setup.CanKillWithoutMafioso && !TrySuggester(out _)) return;
            SetupTargets<MafiaHeadKey>(abilities, TargetFilter.Living(Match).Except(User.Role.Team));
        }

        public override bool Use(IPlayer target)
        {
            if (TrySuggester(out var minion))
            {
                minion.Targets.ForceSet(target);
                Targets.ForceSet(null);

                return true;
            }

            if (Setup.CanKillWithoutMafioso)
            {
                var attack = Attack(priority: Priority);
                attack.Use(target);
            }

            return false;
        }
    }

    public class MafiaHeadSetup : IAbilitySetup
    {
        public bool CanKillWithoutMafioso { get; set; } = true;
    }
}