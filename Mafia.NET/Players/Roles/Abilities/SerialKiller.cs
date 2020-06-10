using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Serial Killer", 5, typeof(SerialKillerSetup))]
    public class SerialKiller : NightEndAbility<SerialKillerSetup>
    {
        public AttackStrength Strength { get; set; } = AttackStrength.Base;

        public void KillRoleBlockers()
        {
            if (Setup.KillsRoleBlockers)
                foreach (var blocker in User.Perks.RoleBlockers)
                    Attack(Strength, Priority).Use(blocker);
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<SerialKillerKey>(abilities, TargetFilter.Living(Match).Except(User));
        }

        public override bool Active()
        {
            return !RoleBlocked || Setup.KillsRoleBlockers;
        }

        public override bool Use()
        {
            KillRoleBlockers();
            return false;
        }

        public override bool Use(IPlayer target)
        {
            KillRoleBlockers();

            if (!RoleBlocked)
            {
                var attack = Attack(Strength, Priority);
                return attack.Use(target);
            }

            return false;
        }

        [RegisterKey]
        protected enum SerialKillerKey
        {
            UserAddMessage,
            UserRemoveMessage,
            UserChangeMessage
        }
    }

    public class SerialKillerSetup : IAbilitySetup
    {
        public bool KillsRoleBlockers { get; set; }
        public bool WinsTiesOverArsonist { get; set; } = false;
    }
}