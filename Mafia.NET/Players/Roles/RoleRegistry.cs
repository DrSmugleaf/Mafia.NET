using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Players.Teams;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class RoleRegistry
    {
        public RoleRegistry(IDictionary<string, RoleEntry> names)
        {
            Ids = names.ToImmutableDictionary();
        }

        public RoleRegistry()
        {
            var ids = new Dictionary<string, RoleEntry>();

            foreach (var yaml in LoadYaml())
            {
                var id = yaml["id"].AsString();
                var team = yaml["team"].AsString();
                var categories = new List<ICategory>();
                var categoryNames = yaml["categories"];

                foreach (var category in (YamlSequenceNode) categoryNames)
                    categories.Add((Category) category.AsString());

                var children = yaml.Children;
                var color = yaml["color"].AsColor();
                var originalColor = children.ContainsKey("original_color") ? yaml["original_color"].AsColor() : color;
                var natural = !children.ContainsKey("natural") || yaml["natural"].AsBool();
                var unique = children.ContainsKey("unique") && yaml["unique"].AsBool();
                var abilities = children.ContainsKey("abilities")
                    ? yaml["abilities"].AsStringList()
                    : new List<string>();

                var defense = AttackStrength.None;
                var detectionImmune = false;
                var roleBlockImmune = false;
                // TODO: Detection and heal profiles
                Func<IPlayer, IHealProfile> healProfile = user => new HealProfile(user);
                if (yaml.Try("perks", out var perks))
                {
                    if (perks.Try("Defense", out var defenseNode))
                        defense = Enum.Parse<AttackStrength>(defenseNode.AsString(), true);

                    if (perks.Try("DetectionImmune", out var detectionNode))
                        detectionImmune = detectionNode.AsBool();

                    if (perks.Try("RoleBlockImmune", out var rbNode))
                        roleBlockImmune = rbNode.AsBool();

                    if (perks.Try("HealProfile", out var profile))
                    {
                        if (profile.AsString() == "No Heal")
                            healProfile = user => new NoHealProfile(user);
                        else
                            throw new ArgumentException($"Unknown heal profile {profile.AsString()}");
                    }
                }

                var role = new RoleEntry(id, team, categories, color, originalColor, natural, unique, abilities,
                    defense,
                    detectionImmune, roleBlockImmune, healProfile);
                ids.Add(id, role);
            }

            Ids = ids.ToImmutableDictionary();
        }

        public IImmutableDictionary<string, RoleEntry> Ids { get; }

        public RoleEntry this[string id] => Ids[id];

        public static List<YamlMappingNode> LoadYaml()
        {
            var yaml = Resource.FromDirectory("Roles", "*.yml");
            return yaml.Select(resource => (YamlMappingNode) resource).ToList();
        }

        public List<RoleEntry> Get()
        {
            return Ids.Values.ToList();
        }

        public List<RoleEntry> Get(params string[] ids)
        {
            var roles = new List<RoleEntry>();

            foreach (var name in ids)
            {
                var role = Ids[name];
                roles.Add(role);
            }

            return roles;
        }

        public IRoleSelector Selector(string id)
        {
            return new RoleSelector(Ids[id]);
        }

        public List<IRoleSelector> Selectors(params string[] ids)
        {
            return Get(ids).Select(role => new RoleSelector(role)).ToList<IRoleSelector>();
        }

        public List<RoleEntry> Category(ICategory category)
        {
            return Ids.Values
                .Where(role => role.Categories.Contains(category))
                .OrderBy(role => role.Id).ToList();
        }

        public List<RoleEntry> Team(ITeam team)
        {
            return Ids.Values
                .Where(role => role.Team == team.Id)
                .OrderBy(role => role.Id).ToList();
        }
    }
}