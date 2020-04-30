using Mafia.NET.Extension;
using Mafia.NET.Resources;
using System.Collections.Generic;
using System.Drawing;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Player.Teams
{
    class Team : ITeam
    {
        public static readonly IReadOnlyDictionary<string, Team> Teams = LoadAll();
        public string Name { get; }
        public Color Tint { get; }

        private Team(string name, Color tint)
        {
            Name = name;
            Tint = tint;
        }

        public static explicit operator Team(string name) => Teams[name];

        private static Dictionary<string, Team> LoadAll()
        {
            var teams = new Dictionary<string, Team>();
            YamlSequenceNode yaml = (YamlSequenceNode)new Resource("teams.yml");

            foreach (var entry in yaml)
            {
                var name = entry["name"].ToString();
                var color = entry["color"];
                var r = color["r"].AsInt();
                var g = color["g"].AsInt();
                var b = color["b"].AsInt();
                var tint = Color.FromArgb(r, g, b);

                var team = new Team(name, tint);
                teams.Add(name, team);
            }

            return teams;
        }
    }
}
