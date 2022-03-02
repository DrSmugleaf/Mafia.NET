using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.Perks
{
    public class PerkEntry : IRegistrable
    {
        public PerkEntry(string id)
        {
            Id = id;
        }

        public AttackStrength Defense { get; set; }
        public bool DetectionImmune { get; set; }
        public bool RoleBlockImmune { get; set; }
        public HealProfileEntry HealProfile { get; set; } = null!;

        public string Id { get; }

        public void SetHeal<T>() where T : IHealProfile
        {
            HealProfile = HealProfileRegistry.Default.Entry<T>();
        }
    }
}