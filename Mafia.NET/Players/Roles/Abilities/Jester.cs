using System.Linq;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Jester", 0, typeof(JesterSetup))]
    public class Jester : NightEndAbility<IJesterSetup>
    {
        public bool UserLynchedToday(out ILynch lynch)
        {
            lynch = (ILynch) User.Match.Graveyard.LynchesToday().FirstOrDefault(death => death.Victim == User);

            return lynch != default;
        }

        public override bool Active()
        {
            return UserLynchedToday(out _) && Setup.RandomGuiltyVoterDies;
        }

        public override bool Use()
        {
            if (!Setup.RandomGuiltyVoterDies ||
                !UserLynchedToday(out var lynch) ||
                lynch.For.Count == 0) return false;

            var random = Match.Random.Next(lynch.For.Count);
            var suicide = lynch.For[random];

            var attack = Attack(AttackStrength.Suicide, 0, false, false);
            attack.Use(suicide);

            return true;
        }
    }

    public interface IJesterSetup : IAbilitySetup
    {
        bool RandomGuiltyVoterDies { get; set; }
    }

    public class JesterSetup : IJesterSetup
    {
        public bool RandomGuiltyVoterDies { get; set; } = true;
    }
}