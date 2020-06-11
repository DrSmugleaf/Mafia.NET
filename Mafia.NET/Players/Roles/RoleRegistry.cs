using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.HealProfiles;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Players.Teams;
using Mafia.NET.Registries;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Roles
{
    public class RoleRegistry : ImmutableRegistry<RoleEntry>
    {
        private static readonly Lazy<RoleRegistry> Lazy = new Lazy<RoleRegistry>(() => new RoleRegistry());

        public RoleRegistry(Dictionary<string, RoleEntry> ids) : base(ids)
        {
        }

        private RoleRegistry() : this(LoadAll())
        {
        }

        public static RoleRegistry Default => Lazy.Value;

        public static List<YamlMappingNode> LoadYaml()
        {
            var yaml = Resource.FromDirectory("Roles", "*.yml");
            return yaml.Select(resource => (YamlMappingNode) resource).ToList();
        }

        public static Dictionary<string, RoleEntry> LoadAll()
        {
            var ids = new Dictionary<string, RoleEntry>();
            foreach (var yaml in LoadYaml())
            {
                var id = yaml["id"].AsString();
                var team = TeamRegistry.Default[yaml["team"].AsString()];
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

                var role = new RoleEntry(
                    id, team, categories, color, originalColor, natural, unique,
                    abilities, defense, detectionImmune, roleBlockImmune, healProfile);
                ids.Add(id, role);
            }

            return ids;
        }

        public List<RoleEntry> Get()
        {
            return Ids.Values.ToList();
        }

        public IRoleSelector Selector(string id)
        {
            return new RoleSelector(Ids[id]);
        }

        public List<IRoleSelector> Selectors(params string[] ids)
        {
            return ids.Select(Selector).ToList();
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
                .Where(role => role.Team == team)
                .OrderBy(role => role.Id).ToList();
        }
    }
}