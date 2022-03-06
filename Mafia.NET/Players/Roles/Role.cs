using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Managers;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.DetectionProfiles;
using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles;

public interface IRole : IColorizable, ILocalizable
{
    string Id { get; }
    Key Name { get; }
    Key Summary { get; }
    Key Goal { get; }
    Key AbilitiesDescriptions { get; }
    ITeam Team { get; }
    IReadOnlyList<ICategory> Categories { get; }
    bool Unique { get; }
    AbilityManager Abilities { get; }
    PerkManager Perks { get; }
    IDetectionProfile DetectionProfile { get; }
    IHealProfile HealProfile { get; }

    void Initialize(IPlayer user);
    void ChangeUser(IPlayer user);
    bool IsCategory(string id);
    IReadOnlyList<Goal> Goals();
    IReadOnlyList<Goal> Enemies();
}

public class Role : IRole
{
    public Role(RoleEntry role)
    {
        Id = role.Id;
        Name = role.Name;
        Summary = role.Summary;
        Goal = role.Goal;
        AbilitiesDescriptions = role.AbilitiesDescription;
        Team = role.Team;
        Categories = role.Categories;
        Color = role.Color;
        Unique = role.Unique;
        Abilities = new AbilityManager();
        Perks = new PerkManager();
        DetectionProfile = new DetectionProfile(null!);
    }

    public string Id { get; }
    public Key Name { get; }
    public Key Summary { get; }
    public Key Goal { get; }
    public Key AbilitiesDescriptions { get; }
    public ITeam Team { get; }
    public IReadOnlyList<ICategory> Categories { get; }
    public Color Color { get; }
    public bool Unique { get; }
    public AbilityManager Abilities { get; }
    public PerkManager Perks { get; }
    public IDetectionProfile DetectionProfile { get; set; }
    public IHealProfile HealProfile { get; set; } = null!;

    public void Initialize(IPlayer user)
    {
        var entry = user.Match.Roles[Id];
        Abilities.Replace(entry.Abilities.Values, user);

        var perks = user.Match.Perks[Id];
        Perks.Defense = perks.Defense;
        Perks.DetectionImmune = perks.DetectionImmune;
        Perks.RoleBlockImmune = perks.RoleBlockImmune;
        HealProfile = perks.HealProfile.Build(user);

        ChangeUser(user);
    }

    public void ChangeUser(IPlayer user)
    {
        foreach (var ability in Abilities.All) ability.User = user;
        Perks.User = user;
        DetectionProfile.User = user;
        HealProfile.User = user;
    }

    public bool IsCategory(string id)
    {
        return Categories.Any(category => category.Id == id);
    }

    public IReadOnlyList<Goal> Goals()
    {
        return Categories.Select(category => category.Goal).ToList();
    }

    public IReadOnlyList<Goal> Enemies()
    {
        return Categories.SelectMany(category => category.Goal.Enemies()).ToList();
    }

    public Text Localize(CultureInfo? culture = null)
    {
        return Name.Localize(culture);
    }

    public override string ToString()
    {
        return Localize().ToString();
    }
}