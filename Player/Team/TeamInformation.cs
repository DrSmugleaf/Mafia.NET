using Mafia.NET.Extension;
using Mafia.NET.Resources;
using System.Collections.Generic;
using System.Drawing;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Player.Team
{
    class TeamInformation
    {
        public string Name { get; }
        public Color Tint { get; }

        private TeamInformation(string name, Color tint)
        {
            Name = name;
            Tint = tint;
        }

        public static IList<TeamInformation> LoadAll()
        {
            var teams = new List<TeamInformation>();
            YamlSequenceNode yaml = new Resource("teams.yml");

            foreach (var entry in yaml)
            {
                var name = entry["name"].ToString();
                var color = entry["color"];
                var r = color["r"].ToInt();
                var g = color["g"].ToInt();
                var b = color["b"].ToInt();
                var tint = Color.FromArgb(r, g, b);

                var team = new TeamInformation(name, tint);
                teams.Add(team);
            }

            return teams;
        }
    }
}
