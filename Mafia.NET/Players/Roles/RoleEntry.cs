using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles
{
    public class RoleEntry : IRegistrable, IColorizable, ILocalizable
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
            AttackStrength defaultDefense,
            bool defaultDetectionImmune,
            bool defaultRoleBlockImmune,
            Func<IPlayer, IHealProfile> defaultHealProfile)
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
            DefaultDefense = defaultDefense;
            DefaultDetectionImmune = defaultDetectionImmune;
            DefaultRoleBlockImmune = defaultRoleBlockImmune;
            DefaultHealProfile = defaultHealProfile;
        }

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
        public AttackStrength DefaultDefense { get; }
        public bool DefaultDetectionImmune { get; }
        public bool DefaultRoleBlockImmune { get; }
        public Func<IPlayer, IHealProfile> DefaultHealProfile { get; }
        public Color Color { get; }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public string Id { get; }

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