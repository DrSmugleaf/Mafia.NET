using System.Collections.Generic;
using System.Drawing;
using Mafia.NET.Extension;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Teams
{
    public interface ITeam
    {
        string Name { get; }
        Color Tint { get; }
    }

    public class Team : ITeam
    {
        public static readonly IReadOnlyDictionary<string, Team> Teams = LoadAll();

        private Team(string name, Color tint)
        {
            Name = name;
            Tint = tint;
        }

        public string Name { get; }
        public Color Tint { get; }

        public static explicit operator Team(string name)
        {
            return Teams[name];
        }

        private static Dictionary<string, Team> LoadAll()
        {
            var teams = new Dictionary<string, Team>();
            var yamlTeams = Resource.FromDirectory("Teams", "*.yml");

            foreach (YamlMappingNode yaml in yamlTeams)
            {
                var name = yaml["name"].ToString();
                var color = yaml["color"].AsColor();
                var team = new Team(name, color);
                teams.Add(name, team);
            }

            return teams;
        }
    }
}