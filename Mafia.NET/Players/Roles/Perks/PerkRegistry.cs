using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Perks
{
    public class PerkRegistry : WritableRegistry<PerkEntry>
    {
        public PerkRegistry(RoleRegistry roles)
        {
            Roles = roles;
        }

        public RoleRegistry Roles { get; }

        public override PerkEntry Create(string id)
        {
            var role = Roles[id];

            return new PerkEntry(id)
            {
                Defense = role.DefaultDefense,
                DetectionImmune = role.DefaultDetectionImmune,
                HealProfile = role.DefaultHealProfile,
                RoleBlockImmune = role.DefaultRoleBlockImmune
            };
        }
    }
}