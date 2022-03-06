using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Registries;
using Mafia.NET.Resources;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Players.Teams;

public class TeamRegistry : ImmutableRegistry<ITeam>
{
    private static readonly Lazy<TeamRegistry> Lazy = new(() => new TeamRegistry());

    public TeamRegistry(Dictionary<string, ITeam> ids) : base(ids)
    {
    }

    private TeamRegistry() : this(LoadAll())
    {
    }

    public static TeamRegistry Default => Lazy.Value;

    public static List<YamlMappingNode> LoadYaml()
    {
        var teams = Resource.FromDirectory("Teams", "*.yml");
        return teams.Select(team => (YamlMappingNode) team).ToList();
    }

    public static Dictionary<string, ITeam> LoadAll()
    {
        var teams = new Dictionary<string, ITeam>();

        foreach (var yaml in LoadYaml())
        {
            var id = yaml["id"].AsString();
            var color = yaml["color"].AsColor();
            var order = yaml["order"].AsInt();
            var team = new Team(id, color, order);
            teams.Add(id, team);
        }

        return teams.OrderBy(team => team.Value.Order).ToDictionary();
    }
}