using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Teams
{
    public interface ITeam : IColorizable, ILocalizable
    {
        string Id { get; }
        Key Name { get; }
        int Order { get; }

        List<RoleEntry> Roles();
        List<IRoleSelector> Selectors(RoleRegistry roles);
    }

    public class Team : ITeam
    {
        public static readonly IImmutableList<Team> All = LoadAll();

        private Team(string id, Color color, int order)
        {
            Id = id;
            Name = new Key($"{id}name");
            Color = color;
            Order = order;
        }

        public string Id { get; }
        public Key Name { get; }
        public Color Color { get; }
        public int Order { get; }

        public List<RoleEntry> Roles()
        {
            return RoleRegistry.Default.Team(this);
        }

        public List<IRoleSelector> Selectors(RoleRegistry roles)
        {
            var selectors = new List<IRoleSelector>();
            
            foreach (var category in Category.Categories.Values)
            {
                var selector = new RoleSelector(roles, category);
                selectors.Add(selector);
            }
            
            var random = new RoleSelector(roles, this);
            selectors.Add(random);

            return selectors;
        }

        public string Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public static explicit operator Team(string id)
        {
            return All.First(team => team.Id == id);
        }

        private static ImmutableList<Team> LoadAll()
        {
            var teams = new List<Team>();
            var yamlTeams = Resource.FromDirectory("Teams", "*.yml");

            foreach (YamlMappingNode yaml in yamlTeams)
            {
                var id = yaml["id"].AsString();
                var color = yaml["color"].AsColor();
                var order = yaml["order"].AsInt();
                var team = new Team(id, color, order);
                teams.Add(team);
            }

            return teams.OrderBy(team => team.Order).ToImmutableList();
        }

        public override string ToString()
        {
            return Localize();
        }
    }
}