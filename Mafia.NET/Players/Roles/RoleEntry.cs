using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles
{
    public class RoleEntry : IColorizable, ILocalizable
    {
        public RoleEntry(
            string id,
            string team,
            IList<ICategory> categories,
            Color color,
            Color originalColor,
            bool natural,
            bool unique,
            IList<string> abilities,
            AttackStrength defense,
            bool detectionImmune,
            bool roleBlockImmune,
            Func<IPlayer, IHealProfile> healProfile)
        {
            Id = id;
            Name = new Key($"{id}name");
            Summary = new Key($"{id}summary");
            Goal = new Key($"{id}goal");
            AbilitiesDescription = new Key($"{id}abilities");
            Team = team;
            Categories = categories.ToImmutableList();
            Color = color;
            OriginalColor = originalColor;
            Natural = natural;
            Unique = unique;
            Abilities = abilities.ToImmutableList();
            Defense = defense;
            DetectionImmune = detectionImmune;
            RoleBlockImmune = roleBlockImmune;
            HealProfile = healProfile;
        }

        public string Id { get; }
        public Key Name { get; }
        public Key Summary { get; }
        public Key Goal { get; }
        public Key AbilitiesDescription { get; }
        public string Team { get; }
        public IImmutableList<ICategory> Categories { get; }
        public Color OriginalColor { get; }
        public bool Natural { get; }
        public bool Unique { get; }
        public IImmutableList<string> Abilities { get; }
        public AttackStrength Defense { get; set; }
        public bool DetectionImmune { get; set; }
        public bool RoleBlockImmune { get; set; }
        public Func<IPlayer, IHealProfile> HealProfile { get; set; }
        public Color Color { get; }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public IRole Build()
        {
            return new Role(this);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}