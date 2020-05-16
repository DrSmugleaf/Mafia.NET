using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Players.Roles;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Teams
{
    public interface ITeam
    {
        string Name { get; }
        Color Color { get; }
        int Order { get; }

        string ColorHtml();
        List<RoleEntry> Roles();
    }

    public class Team : ITeam
    {
        public static readonly IImmutableList<Team> All = LoadAll();

        private Team(string name, Color color, int order)
        {
            Name = name;
            Color = color;
            Order = order;
        }

        public string Name { get; }
        public Color Color { get; }
        public int Order { get; }

        public string ColorHtml()
        {
            return ColorTranslator.ToHtml(Color);
        }

        public List<RoleEntry> Roles()
        {
            return RoleRegistry.Default.Team(this);
        }

        public static explicit operator Team(string name)
        {
            return All.First(team => team.Name == name);
        }

        private static ImmutableList<Team> LoadAll()
        {
            var teams = new List<Team>();
            var yamlTeams = Resource.FromDirectory("Teams", "*.yml");

            foreach (YamlMappingNode yaml in yamlTeams)
            {
                var name = yaml["name"].AsString();
                var color = yaml["color"].AsColor();
                var order = yaml["order"].AsInt();
                var team = new Team(name, color, order);
                teams.Add(team);
            }

            return teams.OrderBy(team => team.Order).ToImmutableList();
        }
    }
}