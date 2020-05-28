using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum VeteranKey
    {
        UserAddMessage,
        UserRemoveMessage
    }

    [RegisterAbility("Veteran", typeof(VeteranSetup))]
    public class Veteran : TownAbility<VeteranSetup>
    {
        public override void Initialize(IPlayer user)
        {
            InitializeBase(user);
            RoleBlockImmune = true;
        }

        public override void Vest()
        {
            if (!TargetManager.Try(out var target) || target != User) return;
            CurrentlyNightImmune = true;
        }

        public override void Revenge()
        {
            if (!TargetManager.Try(out var target) || target != User) return;

            foreach (var attacker in Match.LivingPlayers)
            {
                var ability = attacker.Role.Ability;
                if (!ability.TargetManager.Any(User) ||
                    attacker == User ||
                    ability is Lookout ||
                    ability is Amnesiac ||
                    ability is Coroner ||
                    ability is Janitor) continue;

                PiercingAttack(attacker);
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(User, TargetNotification.Enum<VeteranKey>());
        }
    }

    public class VeteranSetup : ITownSetup, IRandomExcluded, IUsesSetup
    {
        public bool ExcludedFromRandoms { get; set; } = false;
        public int Uses { get; set; } = 2;
    }
}