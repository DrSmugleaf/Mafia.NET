using Mafia.NET.Extension;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Tests.Resources;

[TestFixture]
public class CategoryTest
{
    [Test]
    public void Formatting()
    {
        foreach (var yaml in CategoryRegistry.LoadYaml())
        {
            var children = yaml.Children;

            Assert.That(children, Contains.Key((YamlNode) "id"));
            Assert.That(children["id"].AsString().Length, Is.Positive);

            Assert.That(children, Contains.Key((YamlNode) "goal"));
            var goal = children["goal"].AsString();
            Assert.That(goal.Length, Is.Positive);

            Assert.That(children, Contains.Key((YamlNode) "team"));
            var team = children["team"].AsString();
            Assert.That(team.Length, Is.Positive);
            Assert.NotNull(TeamRegistry.Default[team]);
        }
    }
}