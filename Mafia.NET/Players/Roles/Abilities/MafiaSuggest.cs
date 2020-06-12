using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum MafiaSuggestKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Mafia Suggest", 5, typeof(MafiaSuggestSetup))]
    public class MafiaSuggest : NightEndAbility<MafiaSuggestSetup>
    {
        public MafiaSuggest()
        {
            Strength = AttackStrength.Base;
        }

        public AttackStrength Strength { get; set; }

        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<MafiaSuggestKey>(abilities, TargetFilter.Living(Match).Except(User.Role.Team));
            // TODO: Change messages when alone, teammate messages
        }

        public override bool Use(IPlayer victim)
        {
            var alliedSuggesterAttacked = Match.Graveyard
                .ThreatsOn(victim)
                .Any(threat =>
                    threat.Killer?.Role.Team == User.Role.Team &&
                    threat.Killer?.Abilities.Any<MafiaSuggest>() == true);

            if (alliedSuggesterAttacked) return false;

            User.Crimes.Add(CrimeKey.Trespassing);
            var attack = Attack(Strength, Priority);
            attack.Use(victim);

            return true;
        }
    }

    [RegisterSetup]
    public class MafiaSuggestSetup : IAbilitySetup
    {
    }
}