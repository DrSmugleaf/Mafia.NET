using System;
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
        public Func<IPlayer, IHealProfile> HealProfile { get; set; }

        public string Id { get; }
    }
}